using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.Storefront.AutoRestClients.CatalogModuleApi;
using VirtoCommerce.Storefront.Infrastructure;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Caching;
using VirtoCommerce.Storefront.Model.Catalog;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.Customer.Services;
using VirtoCommerce.Storefront.Model.Inventory.Services;
using VirtoCommerce.Storefront.Model.Pricing.Services;
using VirtoCommerce.Storefront.Model.Services;
using VirtoCommerce.Storefront.Model.Subscriptions.Services;

namespace VirtoCommerce.Storefront.Domain
{
    public class DemoCatalogService : CatalogService, IDemoCatalogService
    {
        public DemoCatalogService(IWorkContextAccessor workContextAccessor,
            ICatalogModuleCategories categoriesApi, ICatalogModuleProducts productsApi,
            ICatalogModuleAssociations associationsApi, ICatalogModuleIndexedSearch searchApi,
            IPricingService pricingService, IMemberService customerService,
            ISubscriptionService subscriptionService, IInventoryService inventoryService,
            IStorefrontMemoryCache memoryCache, IApiChangesWatcher changesWatcher,
            IStorefrontUrlBuilder storefrontUrlBuilder) :
            base(workContextAccessor,
                categoriesApi, productsApi, associationsApi, searchApi,
                pricingService, customerService, subscriptionService, inventoryService,
                memoryCache, changesWatcher,
                storefrontUrlBuilder)
        {
        }

        public async Task<ProductPart[]> GetProductPartsAsync(string productId)
        {
            var products = await GetProductsAsync(new[]
            {
                "cbd8eab4f76b4e34b779d9c59dbc13fe",
                "7ae66c41242c4020a3328d5e841bda49",
                "8e3a763a3cff407b97e2a2f6390b4048",
                "cb8b439491444dad94030be5f551901c",
                "6e37b94d874247e4ab7469e8fefd247e"
            });
            var parts = new[]
            {
                new ProductPart
                {
                    Name = "Case",
                    Image = new Image { Url = "https://raw.githubusercontent.com/VirtoCommerce/vc-demo-theme-b2b/dev/assets/images/mock/case.svg" },
                    Items = products,
                    SelectedItemId = "cbd8eab4f76b4e34b779d9c59dbc13fe"
                },
                new ProductPart
                {
                    Name = "Motherboard",
                    Image = new Image { Url = "https://raw.githubusercontent.com/VirtoCommerce/vc-demo-theme-b2b/dev/assets/images/mock/motherboard.svg" },
                    Items = products,
                    SelectedItemId = "7ae66c41242c4020a3328d5e841bda49"
                },
                new ProductPart
                {
                    Name = "Processor",
                    Image = new Image { Url = "https://raw.githubusercontent.com/VirtoCommerce/vc-demo-theme-b2b/dev/assets/images/mock/processor.svg" },
                    Items = products,
                    SelectedItemId = "8e3a763a3cff407b97e2a2f6390b4048"
                },
                new ProductPart
                {
                    Name = "Memory",
                    Image = new Image { Url = "https://raw.githubusercontent.com/VirtoCommerce/vc-demo-theme-b2b/dev/assets/images/mock/memory.svg" },
                    Items = products,
                    SelectedItemId = "cb8b439491444dad94030be5f551901c"
                },
                new ProductPart
                {
                    Name = "Graphics",
                    Image = new Image { Url = "https://raw.githubusercontent.com/VirtoCommerce/vc-demo-theme-b2b/dev/assets/images/mock/graphics.svg" },
                    Items = products,
                    SelectedItemId = "6e37b94d874247e4ab7469e8fefd247e"
                }
            };
            return parts;
        }
    }
}
