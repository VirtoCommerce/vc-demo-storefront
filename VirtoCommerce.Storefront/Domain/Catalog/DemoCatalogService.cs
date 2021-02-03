using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using VirtoCommerce.Storefront.AutoRestClients.CatalogModuleApi;
using VirtoCommerce.Storefront.AutoRestClients.DemoSolutionFeaturesModuleModuleApi;
using VirtoCommerce.Storefront.AutoRestClients.DemoSolutionFeaturesModuleModuleApi.Models;
using VirtoCommerce.Storefront.Infrastructure;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Caching;
using VirtoCommerce.Storefront.Model.Catalog;
using VirtoCommerce.Storefront.Model.Catalog.Services;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.Common.Caching;
using VirtoCommerce.Storefront.Model.Customer.Services;
using VirtoCommerce.Storefront.Model.Inventory.Services;
using VirtoCommerce.Storefront.Model.Pricing.Services;
using VirtoCommerce.Storefront.Model.Subscriptions.Services;

namespace VirtoCommerce.Storefront.Domain.Catalog
{
    public class DemoCatalogService : CatalogService, IDemoCatalogService
    {
        private readonly IStorefrontMemoryCache _memoryCache;
        private readonly IDemoCatalog _demoCatalogApi;
        private readonly IWorkContextAccessor _workContextAccessor;
        public DemoCatalogService(IWorkContextAccessor workContextAccessor,
            ICatalogModuleCategories categoriesApi, ICatalogModuleProducts productsApi,
            ICatalogModuleAssociations associationsApi, ICatalogModuleIndexedSearch searchApi,
            IPricingService pricingService, IMemberService customerService,
            ISubscriptionService subscriptionService, IInventoryService inventoryService,
            IStorefrontMemoryCache memoryCache, IApiChangesWatcher changesWatcher,
            IStorefrontUrlBuilder storefrontUrlBuilder,
            IDemoCatalog demoCatalogApi) :
            base(workContextAccessor,
                categoriesApi, productsApi, associationsApi, searchApi,
                pricingService, customerService, subscriptionService, inventoryService,
                memoryCache, changesWatcher,
                storefrontUrlBuilder)
        {
            _memoryCache = memoryCache;
            _demoCatalogApi = demoCatalogApi;
            _workContextAccessor = workContextAccessor;
        }

        protected override async Task LoadProductDependencies(List<Product> products, ItemResponseGroup responseGroup, WorkContext workContext)
        {
            await base.LoadProductDependencies(products, responseGroup, workContext);

            if (!products.IsNullOrEmpty())
            {
                foreach(var product in products)
                {
                    if(product.ProductType.EqualsInvariant("Configurable"))
                    {
                        var productParts = await GetProductPartsAsync(product.Id);
                        product.Parts = productParts;
                    }
                }
            }
        }

        public async Task<ProductPart[]> GetProductPartsAsync(string productId)
        {
            var workContext = _workContextAccessor.WorkContext;
            var cacheKey = CacheKey.With(GetType(), nameof(GetProductPartsAsync), productId);

            var searchResult = await _memoryCache.GetOrCreateExclusiveAsync(cacheKey, async (cacheEntry) =>
            {
                cacheEntry.AddExpirationToken(CatalogCacheRegion.CreateChangeToken());
                var searchPartsResultDto = await _demoCatalogApi.SearchAsync(new DemoProductPartSearchCriteria() { ConfiguredProductId = productId, Take = 1000 });
                return searchPartsResultDto;
            });

            var itemsIds = searchResult.Results.SelectMany(x => x.PartItems).Select(x => x.ItemId).Distinct().ToArray();
            var allPartItems = await GetProductsAsync(itemsIds); // Potential recursion

            var productParts = searchResult.Results.OrderBy(x => x.Priority).Select(x => {
                var productPart = x.ToProductPart(workContext.CurrentLanguage.CultureName);
                productPart.Items = x.PartItems.OrderBy(itemDto => itemDto.Priority)
                                    .Select(itemDto =>  allPartItems.FirstOrDefault(p => p.Id.EqualsInvariant(itemDto.ItemId)))
                                    .Where(p=>p != null).ToArray();
                return productPart;
                }).ToArray();            

            return productParts;          
        }

    }
}
