using System.Collections.Generic;
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
            var categories = new Dictionary<string, string>
            {
                { "Cases", "f8457db1-6c48-48dd-9ff0-0d31f3db0d93" },
                { "Motherboard", "db7752b9-4cf0-4df1-98c9-b5fa110439d1" },
                { "CPU", "92adf559-70dd-4d39-a75e-4c9c0c7c4e05" },
                { "Memory", "5554ca91-12b5-42df-bd36-7454683f05d5" },
                { "GPU", "f49f255f-512b-4172-ba3c-cfaa8995c159" },
            };
            var searchResult = await SearchProductsAsync(new ProductSearchCriteria
            {
                Outline = "bea69328-eb20-4da9-ac3d-52e9045c18d2", PageSize = 1000
            });
            var products = searchResult.Products;
            var parts = new[]
            {
                new ProductPart
                {
                    Name = "Case",
                    Image = new Image { Url = "https://raw.githubusercontent.com/VirtoCommerce/vc-demo-theme-b2b/dev/assets/images/mock/case.svg" },
                    Items = products.Where(x => x.CategoryId == categories["Cases"]).ToArray(),
                    SelectedItemId = "baa4931161214690ad51c50787b1ed94"
                },
                new ProductPart
                {
                    Name = "Motherboard",
                    Image = new Image { Url = "https://raw.githubusercontent.com/VirtoCommerce/vc-demo-theme-b2b/dev/assets/images/mock/motherboard.svg" },
                    Items = products.Where(x => x.CategoryId == categories["Motherboard"]).ToArray(),
                    SelectedItemId = "e9de38b73c424db19f319c9538184d03"
                },
                new ProductPart
                {
                    Name = "Processor",
                    Image = new Image { Url = "https://raw.githubusercontent.com/VirtoCommerce/vc-demo-theme-b2b/dev/assets/images/mock/processor.svg" },
                    Items = products.Where(x => x.CategoryId == categories["CPU"]).ToArray(),
                    SelectedItemId = "ec235043d51848249e90ef170c371a1c"
                },
                new ProductPart
                {
                    Name = "Memory",
                    Image = new Image { Url = "https://raw.githubusercontent.com/VirtoCommerce/vc-demo-theme-b2b/dev/assets/images/mock/memory.svg" },
                    Items = products.Where(x => x.CategoryId == categories["Memory"]).ToArray(),
                    SelectedItemId = "dae730451bc745bfa642870bdf22f150"
                },
                new ProductPart
                {
                    Name = "Graphics",
                    Image = new Image { Url = "https://raw.githubusercontent.com/VirtoCommerce/vc-demo-theme-b2b/dev/assets/images/mock/graphics.svg" },
                    Items = products.Where(x => x.CategoryId == categories["GPU"]).ToArray(),
                    SelectedItemId = "5512e3a5201541769e1d81fc5217490c"
                }
            };
            return parts;
        }
    }
}
