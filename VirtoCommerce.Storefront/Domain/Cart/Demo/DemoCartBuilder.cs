using System;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Caching;
using VirtoCommerce.Storefront.Model.Cart;
using VirtoCommerce.Storefront.Model.Cart.Services;
using VirtoCommerce.Storefront.Model.Cart.Validators;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.Marketing.Services;
using VirtoCommerce.Storefront.Model.Services;
using VirtoCommerce.Storefront.Model.Subscriptions.Services;
using VirtoCommerce.Storefront.Model.Tax.Services;

namespace VirtoCommerce.Storefront.Domain.Cart.Demo
{
    public class DemoCartBuilder : CartBuilder
    {
        public DemoCartBuilder(
            IWorkContextAccessor workContextAccessor,
            ICartService cartService,
            ICatalogService catalogSearchService,
            IStorefrontMemoryCache memoryCache,
            IPromotionEvaluator promotionEvaluator,
            ITaxEvaluator taxEvaluator,
            ISubscriptionService subscriptionService
            )
            : base(workContextAccessor, cartService, catalogSearchService, memoryCache, promotionEvaluator, taxEvaluator, subscriptionService)
        {
        }

        public override Task RemoveItemAsync(string lineItemId)
        {
            EnsureCartExists();

            var configureLineItem = Cart.ConfiguredItems.FirstOrDefault(x => x.ConfiguredLineItem?.ProductId.Equals(lineItemId, StringComparison.InvariantCulture) ?? false);

            if (configureLineItem != null)
            {
                var configurablePieces = Cart.Items.Where(x => x.ConfiguredProductId?.Equals(lineItemId, StringComparison.InvariantCulture) ?? false).ToArray();

                foreach (var configuirablePieceLineItem in configurablePieces)
                {
                    Cart.Items.Remove(configuirablePieceLineItem);
                }

                Cart.ConfiguredItems.Remove(configureLineItem);
            }

            return base.RemoveItemAsync(lineItemId);
        }

        public override Task ChangeItemQuantityAsync(ChangeCartItemQty changeItemQty)
        {
            EnsureCartExists();

            var configuredProduct = Cart.ConfiguredItems?.FirstOrDefault(x =>
                x.ConfiguredLineItem?.ProductId.Equals(changeItemQty.LineItemId, StringComparison.InvariantCulture) ?? false);

            if (configuredProduct != null)
            {
                foreach (var lineItem in Cart
                    .Items
                    .Where(x =>
                        !string.IsNullOrEmpty(x.ConfiguredProductId) &&
                        x.ConfiguredProductId.Equals(configuredProduct.ConfiguredLineItem.ProductId)
                    )
                )
                {
                    lineItem.Quantity = changeItemQty.Quantity;
                }
            }

            return base.ChangeItemQuantityAsync(changeItemQty);
        }

        public override async Task<bool> AddItemAsync(AddCartItem addCartItem)
        {
            EnsureCartExists();

            var result = await new AddCartItemValidator(Cart).ValidateAsync(addCartItem, ruleSet: Cart.ValidationRuleSet);

            if (result.IsValid)
            {
                var lineItem = addCartItem.Product.ToLineItem(Cart.Language, addCartItem.Quantity);                
                lineItem.Product = addCartItem.Product;
                lineItem.ConfiguredProductId = addCartItem.ConfiguredProductId;

                if (addCartItem.Price != null)
                {
                    var listPrice = new Money(addCartItem.Price.Value, Cart.Currency);
                    lineItem.ListPrice = listPrice;
                    lineItem.SalePrice = listPrice;
                }

                if (!string.IsNullOrEmpty(addCartItem.Comment))
                {
                    lineItem.Comment = addCartItem.Comment;
                }

                if (!addCartItem.DynamicProperties.IsNullOrEmpty())
                {
                    lineItem.DynamicProperties = new MutablePagedList<DynamicProperty>(addCartItem.DynamicProperties.Select(x => new DynamicProperty
                    {
                        Name = x.Key,
                        Values = new[] { new LocalizedString { Language = Cart.Language, Value = x.Value } }
                    }));
                }

                await AddLineItemAsync(lineItem);
            }

            return result.IsValid;
        }

        protected override async Task AddLineItemAsync(LineItem lineItem)
        {
            if (!string.IsNullOrEmpty(lineItem.ConfiguredProductId))
            {
                lineItem.Id = null;
                Cart.Items.Add(lineItem);
            }
            else
            {
                await base.AddLineItemAsync(lineItem);
            }
        }
    }
}
