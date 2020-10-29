using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using VirtoCommerce.Storefront.Model.Cart.Demo;

namespace VirtoCommerce.Storefront.Model.Cart
{
    public partial class ShoppingCart
    {
        [JsonRequired]
        public LineItem[] UsualItems
        {
            get
            {
                var result = Items.Where(x => !ConfiguredGroups.Any(y => y.Items.Contains(x))).ToArray();

                return result;
            }
        }

        public ICollection<ConfiguredGroup> ConfiguredGroups { get; set; } = new List<ConfiguredGroup>();
    }
}
