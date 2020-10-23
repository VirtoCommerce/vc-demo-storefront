using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using VirtoCommerce.Storefront.AutoRestClients.DynamicAssociationsModuleModuleApi;
using VirtoCommerce.Storefront.Infrastructure;
using VirtoCommerce.Storefront.Model.Caching;
using VirtoCommerce.Storefront.Model.Catalog;
using VirtoCommerce.Storefront.Model.Common.Caching;
using VirtoCommerce.Storefront.Model.Recommendations;
using VirtoCommerce.Storefront.Model.Services;

namespace VirtoCommerce.Storefront.Domain.Recommendations
{
    public class DynamicAssociationsProvider : IRecommendationsProvider
    {
        private readonly ICatalogService _catalogService;
        private readonly IAssociations _associationsApi;
        private readonly IStorefrontMemoryCache _memoryCache;
        private readonly IApiChangesWatcher _apiChangesWatcher;

        public DynamicAssociationsProvider(
            ICatalogService catalogService,
            IAssociations associationsApi,
            IStorefrontMemoryCache memoryCache,
            IApiChangesWatcher apiChangesWatcher
            )
        {
            _catalogService = catalogService;
            _associationsApi = associationsApi;
            _memoryCache = memoryCache;
            _apiChangesWatcher = apiChangesWatcher;
        }

        public string ProviderName => "DynamicAssociations";

        public Task AddEventAsync(IEnumerable<UsageEvent> events)
        {
            return Task.FromResult(true);
        }

        public RecommendationEvalContext CreateEvalContext()
        {
            return new DynamicAssociationsEvalContext();
        }

        public async Task<Product[]> GetRecommendationsAsync(RecommendationEvalContext context)
        {
            var dynamicAssociationsContext = context as DynamicAssociationsEvalContext;

            if (dynamicAssociationsContext == null)
            {
                throw new InvalidCastException(nameof(context));
            }

            var cacheKey = CacheKey.With(GetType(), nameof(GetRecommendationsAsync), context.GetCacheKey());

            return await _memoryCache.GetOrCreateExclusiveAsync(cacheKey, async (cacheEntry) =>
            {
                cacheEntry.AddExpirationToken(RecommendationsCacheRegion.CreateChangeToken());
                cacheEntry.AddExpirationToken(_apiChangesWatcher.CreateChangeToken());

                var result = new List<Product>();
                var recommendedProductIds = await _associationsApi.EvaluateDynamicAssociationsAsync(dynamicAssociationsContext.ToContextDto());

                if (recommendedProductIds != null)
                {
                    result.AddRange(await _catalogService.GetProductsAsync(recommendedProductIds.ToArray(), ItemResponseGroup.Seo | ItemResponseGroup.Outlines | ItemResponseGroup.ItemWithPrices | ItemResponseGroup.ItemWithDiscounts | ItemResponseGroup.Inventory));
                }

                return result.ToArray();
            });
        }
    }
}
