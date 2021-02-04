using System.Collections.Generic;

namespace VirtoCommerce.Storefront.Model.Catalog
{
    public partial class Product
    {
        public ICollection<ProductPart> Parts { get; set; } = new List<ProductPart>();
    }
}
