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

            var configuredGroup = Cart.ConfiguredGroups?.FirstOrDefault(x => x.Id.Equals(lineItemId, StringComparison.InvariantCulture));

            if (configuredGroup != null)
            {
                var groupItems = Cart.Items.Where(x => !string.IsNullOrEmpty(x.ConfiguredGroupId) &&
                       x.ConfiguredGroupId.Equals(configuredGroup.Id, StringComparison.InvariantCulture)).ToArray();

                foreach (var lineItem in groupItems)
                {
                    Cart.Items.Remove(lineItem);
                }

                Cart.ConfiguredGroups.Remove(configuredGroup);
            }

            return base.RemoveItemAsync(lineItemId);
        }

        public override Task ChangeItemQuantityAsync(ChangeCartItemQty changeItemQty)
        {
            EnsureCartExists();

            var configuredGroup = Cart.ConfiguredGroups?.FirstOrDefault(x => x.Id.Equals(changeItemQty.LineItemId, StringComparison.InvariantCulture));

            if (configuredGroup != null)
            {
                configuredGroup.Quantity = changeItemQty.Quantity;

                var groupItems = Cart.Items.Where(x => !string.IsNullOrEmpty(x.ConfiguredGroupId) &&
                       x.ConfiguredGroupId.Equals(configuredGroup.Id, StringComparison.InvariantCulture)).ToArray();

                foreach (var lineItem in groupItems)
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
                lineItem.ConfiguredGroupId = addCartItem.ConfiguredGroupId;

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
            var existingLineItem = Cart.Items.FirstOrDefault(li => li.ProductId.EqualsInvariant(lineItem.ProductId)
                                                                && (li.ConfiguredGroupId?.EqualsInvariant(lineItem.ConfiguredGroupId) ?? true));

            if (existingLineItem != null)
            {
                await ChangeItemQuantityAsync(existingLineItem, existingLineItem.Quantity + Math.Max(1, lineItem.Quantity));
                await ChangeItemPriceAsync(existingLineItem, new ChangeCartItemPrice { LineItemId = existingLineItem.Id, NewPrice = lineItem.ListPrice.Amount });
                existingLineItem.Comment = lineItem.Comment;
                existingLineItem.DynamicProperties = lineItem.DynamicProperties;
            }
            else
            {
                lineItem.Id = null;
                Cart.Items.Add(lineItem);
            }
        }
    }
}
