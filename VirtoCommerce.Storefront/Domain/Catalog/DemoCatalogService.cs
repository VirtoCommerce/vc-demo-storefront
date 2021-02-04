using System;
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
using VirtoCommerce.Storefront.Model.Catalog.Demo;
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
        private readonly IApiChangesWatcher _apiChangesWatcher;
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
            _apiChangesWatcher = changesWatcher;
        }

        protected override async Task LoadProductDependencies(List<Product> products, ItemResponseGroup responseGroup, WorkContext workContext)
        {           
            if (!products.IsNullOrEmpty())
            {
                foreach (var product in products.Where(product => product.ProductType.EqualsInvariant(ProductTypes.Configurable)))
                {
                    var productParts = await GetProductPartsAsync(product.Id);
                    product.Parts = productParts;
                }
            }

            await base.LoadProductDependencies(products, responseGroup, workContext);
        }

        public async Task<ProductPart[]> GetProductPartsAsync(string productId)
        {
            var workContext = _workContextAccessor.WorkContext;
            var cacheKey = CacheKey.With(GetType(), nameof(GetProductPartsAsync), productId);

            var searchResult = await _memoryCache.GetOrCreateExclusiveAsync(cacheKey, async (cacheEntry) =>
            {
                cacheEntry.AddExpirationToken(CatalogCacheRegion.CreateChangeToken());
                cacheEntry.AddExpirationToken(_apiChangesWatcher.CreateChangeToken());
                var searchPartsResultDto = await _demoCatalogApi.SearchAsync(new DemoProductPartSearchCriteria { ConfiguredProductId = productId, Take = int.MaxValue });
                return searchPartsResultDto;
            });

            var partItemIds = searchResult.Results?
                .Where(x => x.PartItems != null).SelectMany(x => x.PartItems).Select(x => x.ItemId).Distinct()
                .ToArray();

            var allPartItems = !partItemIds.IsNullOrEmpty() ? await GetProductsAsync(partItemIds) : null; // Potential recursion

            ProductPart ConvertDtoToProductPartAndAttachItsItems(DemoProductPart x, WorkContext workContext, Product[] allPartItems)
            {
                var productPart = x.ToProductPart(workContext.CurrentLanguage.CultureName);
                productPart.Items = x.PartItems?
                    .OrderBy(partItemInfo => partItemInfo.Priority)
                    .Select(partItemInfo => allPartItems?.FirstOrDefault(product => product.Id.EqualsInvariant(partItemInfo.ItemId)))
                    .Where(product => product != null)
                    .ToArray() ?? Array.Empty<Product>();
                return productPart;
            }

            var productParts = searchResult.Results?.OrderBy(x => x.Priority).Select(x => ConvertDtoToProductPartAndAttachItsItems(x, workContext, allPartItems))
                                .ToArray<ProductPart>() ?? Array.Empty<ProductPart>();

            return productParts;          
        }
    }
}
