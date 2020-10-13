using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.Storefront.AutoRestClients.CatalogModuleApi;
using VirtoCommerce.Storefront.Infrastructure;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Caching;
using VirtoCommerce.Storefront.Model.Catalog;
using VirtoCommerce.Storefront.Model.Catalog.Services;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.Customer.Services;
using VirtoCommerce.Storefront.Model.Inventory.Services;
using VirtoCommerce.Storefront.Model.Pricing.Services;
using VirtoCommerce.Storefront.Model.Subscriptions.Services;

namespace VirtoCommerce.Storefront.Domain.Catalog
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

            var casesProducts = products.Where(x => x.CategoryId == categories["Cases"]).ToArray();
            var motherboardsProducts = products.Where(x => x.CategoryId == categories["Motherboard"]).ToArray();
            var processorProducts = products.Where(x => x.CategoryId == categories["CPU"]).ToArray();
            var memoryProducts = products.Where(x => x.CategoryId == categories["Memory"]).ToArray();
            var gpuProducts = products.Where(x => x.CategoryId == categories["GPU"]).ToArray();

            var casesProductPart = TryGetProductPartByCategoryId(categories["Cases"]);
            casesProductPart.Items = casesProducts;
            casesProductPart.SelectedItemId = casesProducts.FirstOrDefault()?.Id;

            var motherboardProductPart = TryGetProductPartByCategoryId(categories["Motherboard"]);
            motherboardProductPart.Items = motherboardsProducts;
            motherboardProductPart.SelectedItemId = motherboardsProducts.FirstOrDefault()?.Id;

            var cpuProductPart = TryGetProductPartByCategoryId(categories["CPU"]);
            cpuProductPart.Items = processorProducts;
            cpuProductPart.SelectedItemId = processorProducts.FirstOrDefault()?.Id;

            var memoryProductPart = TryGetProductPartByCategoryId(categories["Memory"]);
            memoryProductPart.Items = memoryProducts;
            memoryProductPart.SelectedItemId = memoryProducts.FirstOrDefault()?.Id;

            var gpuProductPart = TryGetProductPartByCategoryId(categories["GPU"]);
            gpuProductPart.Items = gpuProducts;
            gpuProductPart.SelectedItemId = gpuProducts.FirstOrDefault()?.Id;

            var parts = new[]
            {
                casesProductPart,
                motherboardProductPart,
                cpuProductPart,
                memoryProductPart,
                gpuProductPart,
            };

            return parts.OrderBy(x => x.Name).ToArray();
        }


        public ProductPart TryGetProductPartByCategoryId(string categoryId)
        {
            var result = new Dictionary<string, ProductPart>
            {
                {
                    "f8457db1-6c48-48dd-9ff0-0d31f3db0d93",
                    new ProductPart
                    {
                        Name = "Case",
                        Image = new Image { Url = "https://raw.githubusercontent.com/VirtoCommerce/vc-demo-theme-b2b/dev/assets/images/mock/case.svg" },
                    }
                },
                {
                    "db7752b9-4cf0-4df1-98c9-b5fa110439d1",
                    new ProductPart
                    {
                        Name = "Motherboard",
                        Image = new Image { Url = "https://raw.githubusercontent.com/VirtoCommerce/vc-demo-theme-b2b/dev/assets/images/mock/motherboard.svg" },
                    }
                },
                {
                    "92adf559-70dd-4d39-a75e-4c9c0c7c4e05",
                    new ProductPart
                    {
                        Name = "Processor",
                        Image = new Image { Url = "https://raw.githubusercontent.com/VirtoCommerce/vc-demo-theme-b2b/dev/assets/images/mock/processor.svg" },
                    }
                },
                {
                    "5554ca91-12b5-42df-bd36-7454683f05d5",
                    new ProductPart
                    {
                        Name = "Memory",
                        Image = new Image { Url = "https://raw.githubusercontent.com/VirtoCommerce/vc-demo-theme-b2b/dev/assets/images/mock/memory.svg" },
                    }
                },
                {
                    "f49f255f-512b-4172-ba3c-cfaa8995c159",
                    new ProductPart
                    {
                        Name = "Graphics",
                        Image = new Image { Url = "https://raw.githubusercontent.com/VirtoCommerce/vc-demo-theme-b2b/dev/assets/images/mock/graphics.svg" },
                    }
                },
            };

            return result.ContainsKey(categoryId) ? result[categoryId] : null;
        }
    }
}
