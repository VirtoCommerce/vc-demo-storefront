using System;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Caching;
using VirtoCommerce.Storefront.Model.Cart;
using VirtoCommerce.Storefront.Model.Cart.Demo;
using VirtoCommerce.Storefront.Model.Cart.Services;
using VirtoCommerce.Storefront.Model.Cart.Validators;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.Marketing.Services;
using VirtoCommerce.Storefront.Model.Security;
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

        public override async Task ClearAsync()
        {
            await base.ClearAsync();
            Cart.ConfiguredGroups.Clear();
        }

        public override async Task MergeWithCartAsync(ShoppingCart cart)
        {
            EnsureCartExists();

            // Clone source cart to prevent its damage
            cart = (ShoppingCart)cart.Clone();

            // Reset primary keys for all aggregated entities before merge
            // to prevent insertions same Ids for target cart.
            // Exclude user because it might be the current one.
            // Exclude configuration groups because we need it to build correct relations
            var entities = cart.GetFlatObjectsListWithInterface<IEntity>();
            foreach (var entity in entities.Where(x => !(x is User || x is ConfiguredGroup)).ToList())
            {
                entity.Id = null;
            }

            foreach (var configuredGroup in cart.ConfiguredGroups)
            {
                var configuredGroupId = configuredGroup.Id;
                configuredGroup.Id = Guid.NewGuid().ToString("N");

                foreach (var item in cart.Items.Where(x => x.ConfiguredGroupId == configuredGroupId))
                {
                    item.ConfiguredGroupId = configuredGroup.Id;
                    configuredGroup.Items.Add(item);
                }

                Cart.ConfiguredGroups.Add(configuredGroup);
            }

            foreach (var lineItem in cart.Items)
            {
                await AddLineItemAsync(lineItem);
            }

            foreach (var coupon in cart.Coupons)
            {
                await AddCouponAsync(coupon.Code);
            }

            foreach (var shipment in cart.Shipments)
            {
                await AddOrUpdateShipmentAsync(shipment);
            }

            foreach (var payment in cart.Payments)
            {
                await AddOrUpdatePaymentAsync(payment);
            }
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
