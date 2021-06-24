using System;
using System.Collections.Generic;

namespace VirtoCommerce.Storefront.Model.Catalog
{
    public partial class Product
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public ICollection<ProductPart> Parts { get; set; } = new List<ProductPart>();
    }
}
