using System;
using System.Threading.Tasks;
using Microsoft.Rest;
using VirtoCommerce.Storefront.AutoRestClients.CustomerModuleApi;
using VirtoCommerce.Storefront.Infrastructure;
using VirtoCommerce.Storefront.Model.Caching;

namespace VirtoCommerce.Storefront.Domain
{
    public class DemoMemberService : MemberService
    {
        private readonly ICustomerModule _customerApi;
        private readonly string NO_CONTENT = "NoContent";

        public DemoMemberService(ICustomerModule customerApi, IStorefrontMemoryCache memoryCache, IApiChangesWatcher changesWatcher)
            : base(customerApi, memoryCache, changesWatcher)
        {
            _customerApi = customerApi;
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
    }
}
