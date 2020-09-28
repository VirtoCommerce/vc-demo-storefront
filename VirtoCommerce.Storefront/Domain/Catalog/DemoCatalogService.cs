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
                "baa4931161214690ad51c50787b1ed94",
                "e9de38b73c424db19f319c9538184d03",
                "ec235043d51848249e90ef170c371a1c",
                "dae730451bc745bfa642870bdf22f150",
                "5512e3a5201541769e1d81fc5217490c"
            });
            var parts = new[]
            {
                new ProductPart
                {
                    Name = "Case",
                    Image = new Image { Url = "https://raw.githubusercontent.com/VirtoCommerce/vc-demo-theme-b2b/dev/assets/images/mock/case.svg" },
                    Items = products,
                    SelectedItemId = "baa4931161214690ad51c50787b1ed94"
                },
                new ProductPart
                {
                    Name = "Motherboard",
                    Image = new Image { Url = "https://raw.githubusercontent.com/VirtoCommerce/vc-demo-theme-b2b/dev/assets/images/mock/motherboard.svg" },
                    Items = products,
                    SelectedItemId = "e9de38b73c424db19f319c9538184d03"
                },
                new ProductPart
                {
                    Name = "Processor",
                    Image = new Image { Url = "https://raw.githubusercontent.com/VirtoCommerce/vc-demo-theme-b2b/dev/assets/images/mock/processor.svg" },
                    Items = products,
                    SelectedItemId = "ec235043d51848249e90ef170c371a1c"
                },
                new ProductPart
                {
                    Name = "Memory",
                    Image = new Image { Url = "https://raw.githubusercontent.com/VirtoCommerce/vc-demo-theme-b2b/dev/assets/images/mock/memory.svg" },
                    Items = products,
                    SelectedItemId = "dae730451bc745bfa642870bdf22f150"
                },
                new ProductPart
                {
                    Name = "Graphics",
                    Image = new Image { Url = "https://raw.githubusercontent.com/VirtoCommerce/vc-demo-theme-b2b/dev/assets/images/mock/graphics.svg" },
                    Items = products,
                    SelectedItemId = "5512e3a5201541769e1d81fc5217490c"
                }
            };
            return parts;
        }
    }
}
