using System;

namespace VirtoCommerce.Storefront.Model.Catalog
{
    public class ProductPart
    {
        public ProductPart()
        {
            Items = Array.Empty<Product>();
        }

        public string Name { get; set; }

        public Image Image { get; set; }

        public Product[] Items { get; set; }

        public string SelectedItemId { get; set; }
    }
}
