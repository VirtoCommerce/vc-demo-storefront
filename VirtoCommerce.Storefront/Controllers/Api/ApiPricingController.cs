using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using VirtoCommerce.Storefront.Infrastructure;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Cart;
using VirtoCommerce.Storefront.Model.Catalog;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.Marketing.Services;
using VirtoCommerce.Storefront.Model.Pricing.Services;
using VirtoCommerce.Storefront.Model.Services;

namespace VirtoCommerce.Storefront.Controllers.Api
{
    [StorefrontApiRoute("pricing")]
    [ResponseCache(CacheProfileName = "None")]
    public class ApiPricingController : StorefrontControllerBase
    {
        private readonly IWorkContextAccessor _workContextAccessor;
        private readonly ICatalogService _catalogService;
        private readonly IPricingService _pricingService;
        private readonly IPromotionEvaluator _promotionEvaluator;

        public ApiPricingController(IWorkContextAccessor workContextAccessor, IStorefrontUrlBuilder urlBuilder,
            ICatalogService catalogService, IPromotionEvaluator promotionEvaluator, IPricingService pricingService)
            : base(workContextAccessor, urlBuilder)
        {
            _workContextAccessor = workContextAccessor;
            _catalogService = catalogService;
            _pricingService = pricingService;
            _promotionEvaluator = promotionEvaluator;
        }

        // POST: storefrontapi/pricing/actualprices
        [HttpPost("actualprices")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult<ProductPrice[]>> GetActualProductPrices([FromBody] Product[] products)
        {
            if (products != null && products.Any())
            {
                //Evaluate products prices
                await _pricingService.EvaluateProductPricesAsync(products, WorkContext);

                var retVal = products.Select(x => x.Price).ToArray();

                return retVal;
            }
            return Ok();
        }

        // POST: storefrontapi/pricing/total
        [HttpPost("total")]
        public async Task<ActionResult<ProductPrice>> GetProductsTotal([FromBody] AddCartItem[] items)
        {
            var currency = _workContextAccessor.WorkContext.CurrentCurrency;
            var productTotal = new ProductTotal(currency);

            // Based on https://github.com/VirtoCommerce/vc-module-cart/blob/05dac9453f375f015bbea57e3f95f4c87a62ec74/src/VirtoCommerce.CartModule.Data/Services/DefaultShoppingCartTotalsCalculator.cs#L57-L68
            if (!items.IsNullOrEmpty())
            {
                var products = await _catalogService.GetProductsAsync(items.Select(x => x.ProductId).ToArray(), ItemResponseGroup.ItemWithPrices);

                if (products != null && products.Any())
                {
                    var productsById = products.ToDictionary(x => x.Id, x => x);

                    foreach (var item in items)
                    {
                        item.Product = productsById[item.ProductId];
                    }

                    // Original price without rounding stored in InternalAmount
                    var subTotal = items.Sum(x => x.Product.Price.ListPrice.InternalAmount * x.Quantity);
                    var subTotalWithTax = items.Sum(x => x.Product.Price.ListPriceWithTax.InternalAmount * x.Quantity);
                    var discountTotal = items.Sum(x => x.Product.Price.DiscountAmount.InternalAmount);
                    var discountTotalWithTax = items.Sum(x => x.Product.Price.DiscountAmountWithTax.InternalAmount);
                    var taxTotal = items.Sum(x => x.Product.TaxTotal.InternalAmount);

                    productTotal.SubTotal = new Money(Math.Round(subTotal, 2, MidpointRounding.AwayFromZero), currency);
                    productTotal.SubTotalWithTax = new Money(Math.Round(subTotalWithTax, 2, MidpointRounding.AwayFromZero), currency);
                    productTotal.DiscountTotal = new Money(Math.Round(discountTotal, 2, MidpointRounding.AwayFromZero), currency);
                    productTotal.DiscountTotalWithTax = new Money(Math.Round(discountTotalWithTax, 2, MidpointRounding.AwayFromZero), currency);
                    productTotal.TaxTotal = new Money(Math.Round(taxTotal, 2, MidpointRounding.AwayFromZero), currency);
                    productTotal.Total = new Money(productTotal.SubTotal.Amount - productTotal.DiscountTotal.Amount, currency);
                    productTotal.TotalWithTax = new Money(productTotal.Total.Amount + productTotal.TaxTotal.Amount, currency);
                }
            }

            return Ok(productTotal);
        }
    }
}
