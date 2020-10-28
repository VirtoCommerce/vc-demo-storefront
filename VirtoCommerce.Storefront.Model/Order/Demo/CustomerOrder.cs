using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using VirtoCommerce.Storefront.Model.Order.Demo;

namespace VirtoCommerce.Storefront.Model.Order
{
    public partial class CustomerOrder
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
