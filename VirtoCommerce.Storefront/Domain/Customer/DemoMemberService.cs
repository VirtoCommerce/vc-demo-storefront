using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Rest;
using Newtonsoft.Json.Linq;
using VirtoCommerce.Storefront.AutoRestClients.CustomerModuleApi;
using VirtoCommerce.Storefront.AutoRestClients.DemoCustomerSegmentsModuleModuleApi;
using VirtoCommerce.Storefront.Infrastructure;
using VirtoCommerce.Storefront.Model.Caching;
using VirtoCommerce.Storefront.Model.Common.Caching;
using VirtoCommerce.Storefront.Model.Customer;
using VirtoCommerce.Storefront.Model.Customer.Services;

namespace VirtoCommerce.Storefront.Domain
{
    public class DemoMemberService : MemberService, IDemoMemberService
    {
        private readonly ICustomerModule _customerApi;
        private readonly IDemoSearch _demoSearchApi;
        private readonly IStorefrontMemoryCache _memoryCache;
        private readonly IApiChangesWatcher _apiChangesWatcher;
        private readonly string NO_CONTENT = "NoContent";

        public DemoMemberService(ICustomerModule customerApi, IDemoSearch demoSearchApi, IStorefrontMemoryCache memoryCache, IApiChangesWatcher apiChangesWatcher)
            : base(customerApi, memoryCache, apiChangesWatcher)
        {
            _customerApi = customerApi;
            _demoSearchApi = demoSearchApi;
            _memoryCache = memoryCache;
            _apiChangesWatcher = apiChangesWatcher;
        }

        public async Task<IDictionary<string, object>> GetMemberIndexByIdAsync(string memberId)
        {
            ValidateParameters(memberId);

            var cacheKey = CacheKey.With(GetType(), "GetMemberIndexByIdAsync", memberId);
            var result = await _memoryCache.GetOrCreateExclusiveAsync(cacheKey, async (cacheEntry) =>
            {
                var indexDto = await _demoSearchApi.GetDocumentIndexAsyncAsync(nameof(Member), memberId);

                cacheEntry.AddExpirationToken(CustomerCacheRegion.CreateChangeToken(memberId));
                cacheEntry.AddExpirationToken(_apiChangesWatcher.CreateChangeToken());
                return indexDto;
            });
            return result;
        }

        public override async Task<Contact> GetContactByIdAsync(string contactId)
        {
            var result = await base.GetContactByIdAsync(contactId);
            var indexDocument = await GetMemberIndexByIdAsync(contactId);
            if (indexDocument != null)
            {
                var groupsField = indexDocument["groups"];
                // This conversion is required because returned IDictionary contains deserialized arrays as JArrays
                string[] groups;
                if (groupsField is JArray groupsArray)
                {
                    groups = groupsArray.ToObject<string[]>();
                }
                else
                {
                    groups = groupsField != null ? new[] { groupsField as string } : Array.Empty<string>();
                }

                result.UserGroups = result.UserGroups.Concat(groups).Distinct().ToArray();
            }
            return result;
        }

        public override async Task DeleteContactAsync(string contactId)
        {
            try
            {
                await _customerApi.DeleteContactsAsync(new[] { contactId });
            }
            catch (HttpOperationException ex)
            {
                /* Our AutoRestClient throws exception on NoContent status code.
                In order not to touch the customer module, we check that the exception status is 'NoContent'
                and in this case we continue execution
                */
                if (!ex.Message.Contains(NO_CONTENT, StringComparison.InvariantCulture))
                {
                    throw;
                }
            }

            //Invalidate cache
            CustomerCacheRegion.ExpireMember(contactId);
        }

        private static void ValidateParameters(string memberId)
        {
            if (memberId == null)
            {
                throw new ArgumentNullException(nameof(memberId));
            }
        }
    }
}
