using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Cart;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.Security;

namespace VirtoCommerce.Storefront.Domain.Cart.Demo
{
    public static class DemoCartConverter
    {
        public static ShoppingCart ToDemoShoppingCart(
            this AutoRestClients.CartModuleApi.Models.ShoppingCart cartDto,
            Currency currency,
            Language language,
            User user
            )
        {
            var cart = cartDto.ToShoppingCart(currency, language, user);

            return cart;
        }
    }
}
