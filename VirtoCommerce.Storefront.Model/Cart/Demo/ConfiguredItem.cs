using System.Collections.Generic;
using VirtoCommerce.Storefront.Model.Catalog;

namespace VirtoCommerce.Storefront.Model.Cart.Demo
{
    public class ConfiguredItem
    {
        public ConfiguredItem()
        {
            Parts = new List<ProductPart>();
        }

        public LineItem ConfiguredLineItem { get; set; }

        public ICollection<ProductPart> Parts { get; set; }
    }
}
