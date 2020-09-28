using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using FluentValidation;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Cart;
using VirtoCommerce.Storefront.Model.Cart.Validators;
using VirtoCommerce.Storefront.Model.Common;

namespace VirtoCommerce.Storefront.Controllers.Api
{    
    public partial class ApiCartController 
    {
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

                var validationResult = await new CartValidator(_cartService).ValidateAsync(cart, ruleSet: "default,strict");

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
    }
}
