using System;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Caching;
using VirtoCommerce.Storefront.Model.Cart.Services;
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

            var configureLineItem = Cart.ConfiguredItems.FirstOrDefault(x => x.ConfiguredLineItem.Id.Equals(lineItemId, StringComparison.InvariantCulture));

            if (configureLineItem != null)
            {
                foreach (var configuirablePieceLineItem in Cart.Items.Where(x => x.ConfiguredProductId.Equals(lineItemId, StringComparison.InvariantCulture)))
                {
                    Cart.Items.Remove(configuirablePieceLineItem);
                }
            }

            return base.RemoveItemAsync(lineItemId);
        }
    }
}
