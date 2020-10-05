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

        public DemoMemberService(ICustomerModule customerApi, IStorefrontMemoryCache memoryCache, IApiChangesWatcher changesWatcher) : base(customerApi, memoryCache, changesWatcher)
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
                if (!ex.Message.Contains(NO_CONTENT))
                {
                    throw;
                }
            }

            //Invalidate cache
            CustomerCacheRegion.ExpireMember(contactId);
        }
    }
}
