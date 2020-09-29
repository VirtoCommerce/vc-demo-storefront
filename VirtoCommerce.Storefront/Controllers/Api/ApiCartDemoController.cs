using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using FluentValidation;
using VirtoCommerce.Storefront.Infrastructure;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Cart;
using VirtoCommerce.Storefront.Model.Cart.Services;
using VirtoCommerce.Storefront.Model.Cart.Validators;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.Common.Exceptions;
using VirtoCommerce.Storefront.Model.Services;


namespace VirtoCommerce.Storefront.Controllers.Api
{
    [StorefrontApiRoute("cartdemo")]
    [ResponseCache(CacheProfileName = "None")]
    public class ApiCartDemoController : StorefrontControllerBase
    {
        private readonly ICartBuilder _cartBuilder;
        private readonly ICatalogService _catalogService;        
        public ApiCartDemoController(IWorkContextAccessor workContextAccessor, ICatalogService catalogService, ICartBuilder cartBuilder,
                                 IStorefrontUrlBuilder urlBuilder)
            : base(workContextAccessor, urlBuilder)
        {
            _cartBuilder = cartBuilder;
            _catalogService = catalogService;
        }


        // POST: storefrontapi/cart/items/bulk
        [HttpPost("items/bulk")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult<AddItemsToCartResult>> AddItemsToCart([FromBody] string[] productIds)
        {
            EnsureCartExists();

            //Need lock to prevent concurrent access to same cart
            using (await AsyncLock.GetLockByKey(WorkContext.CurrentCart.Value.GetCacheKey()).LockAsync())
            {
                var products = await _catalogService.GetProductsAsync(productIds, Model.Catalog.ItemResponseGroup.ItemSmall | Model.Catalog.ItemResponseGroup.ItemWithPrices | Model.Catalog.ItemResponseGroup.Inventory);               
                var cartBuilder = await LoadOrCreateCartAsync();
                var cart = _cartBuilder.Cart;

                foreach (var productId in productIds)
                {
                    await cartBuilder.AddItemAsync(new AddCartItem { ProductId = productId, Quantity = 1, Product = products.First(x=>x.Id == productId) });
                }

                var validationResult = await new CartDemoValidator().ValidateAsync(cart, ruleSet: "default,strict");

                if (validationResult.IsValid)
                {
                    await cartBuilder.SaveAsync();
                }

                var result = new AddItemsToCartResult
                {
                    IsSuccess = validationResult.IsValid,
                    ErrorCodes = validationResult.Errors.Select(x => x.ErrorCode).Distinct().ToArray()
                };

                return result;
            }
        }

        private void EnsureCartExists()
        {
            if (WorkContext.CurrentCart.Value == null)
            {
                throw new StorefrontException("Cart not found");
            }
        }

        private async Task<ICartBuilder> LoadOrCreateCartAsync(string cartName = null, string type = null)
        {
            //Need to try load fresh cart from cache or service to prevent parallel update conflict
            //because WorkContext.CurrentCart may contains old cart
            await _cartBuilder.LoadOrCreateNewTransientCartAsync(cartName ?? WorkContext.CurrentCart.Value.Name, WorkContext.CurrentStore, WorkContext.CurrentUser, WorkContext.CurrentLanguage, WorkContext.CurrentCurrency, type);
            return _cartBuilder;
        }
    }
}
