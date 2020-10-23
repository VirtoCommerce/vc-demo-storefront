using System.Collections.Generic;
using VirtoCommerce.Storefront.Model.Cart.Demo;

namespace VirtoCommerce.Storefront.Model.Cart
{
    public partial class ShoppingCart
    {
        public ICollection<ConfiguredGroup> ConfiguredGroups { get; set; } = new List<ConfiguredGroup>();
    }
}
