using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;
using PagedList.Core;
using VirtoCommerce.Storefront.AutoRestClients.CartModuleApi;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Caching;
using VirtoCommerce.Storefront.Model.Cart;
using VirtoCommerce.Storefront.Model.Catalog;
using VirtoCommerce.Storefront.Model.Catalog.Services;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.Common.Caching;
using VirtoCommerce.Storefront.Model.Security;
using VirtoCommerce.Storefront.Model.Services;

namespace VirtoCommerce.Storefront.Domain.Cart.Demo
{
    public class DemoCartService : CartService
    {
        private readonly IStorefrontMemoryCache _memoryCache;
        private readonly ICartModule _cartApi;
        private readonly IWorkContextAccessor _workContextAccessor;
        private readonly UserManager<User> _userManager;
        private readonly IDemoCatalogService _demoCatalogService;
        private readonly ICatalogService _catalogService;

        public DemoCartService(
            ICartModule cartModule,
            IWorkContextAccessor workContextAccessor,
            IStorefrontMemoryCache memoryCache,
            UserManager<User> userManager,
            IDemoCatalogService demoCatalogService,
            ICatalogService catalogService)
            : base(cartModule, workContextAccessor, memoryCache, userManager)
        {
            _userManager = userManager;
            _workContextAccessor = workContextAccessor;
            _cartApi = cartModule;
            _memoryCache = memoryCache;
            _demoCatalogService = demoCatalogService;
            _catalogService = catalogService;
        }

        public override async Task<IPagedList<ShoppingCart>> SearchCartsAsync(CartSearchCriteria criteria)
        {
            if (criteria == null)
            {
                throw new ArgumentNullException(nameof(criteria));
            }

            var cacheKey = CacheKey.With(GetType(), "SearchCartsAsync", criteria.GetCacheKey());
            return await _memoryCache.GetOrCreateExclusiveAsync(cacheKey, async cacheEntry =>
            {
                cacheEntry.AddExpirationToken(CartCacheRegion.CreateCustomerChangeToken(criteria.Customer?.Id));

                var resultDto = await _cartApi.SearchShoppingCartAsync(criteria.ToSearchCriteriaDto());
                var result = new List<ShoppingCart>();
                foreach (var cartDto in resultDto.Results)
                {
                    var currency = _workContextAccessor.WorkContext.AllCurrencies.FirstOrDefault(x => x.Equals(cartDto.Currency));
                    var language = string.IsNullOrEmpty(cartDto.LanguageCode) ? Language.InvariantLanguage : new Language(cartDto.LanguageCode);
                    var user = await _userManager.FindByIdAsync(cartDto.CustomerId) ?? criteria.Customer;

                    var cart = cartDto.ToShoppingCart(currency, language, user);

                    await FillProductPartsOfGroupsAsync(cart);

                    result.Add(cart);
                }
                return new StaticPagedList<ShoppingCart>(result, criteria.PageNumber, criteria.PageSize, resultDto.TotalCount.Value);
            });
        }

        protected virtual async Task FillProductPartsOfGroupsAsync(ShoppingCart cart)
        {
            if (cart.ConfiguredGroups.IsNullOrEmpty())
            {
                return;
            }

            var groupProductsIds = cart.ConfiguredGroups.Select(x => x.ProductId).ToArray();
            var groupProducts = await _catalogService.GetProductsAsync(groupProductsIds, ItemResponseGroup.None);

            foreach (var group in cart.ConfiguredGroups)
            {
                var product = groupProducts.FirstOrDefault(x => x.Id.Equals(group.ProductId, StringComparison.InvariantCulture));
                group.Product = product;
            }
        }
    }
}
