using System.Collections.Generic;
using VirtoCommerce.Storefront.Model.Cart.Demo;

namespace VirtoCommerce.Storefront.Model.Cart
{
    public partial class ShoppingCart
    {

        public ICollection<ConfiguredItem> ConfiguredItems { get; set; } = new List<ConfiguredItem>();
    }
}
