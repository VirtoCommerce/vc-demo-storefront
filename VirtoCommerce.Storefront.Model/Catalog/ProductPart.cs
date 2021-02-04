using System;

namespace VirtoCommerce.Storefront.Model.Catalog
{
    public class ProductPart
    {
        public ProductPart()
        {
            Items = Array.Empty<Product>();
        }

        public string Id { get; set; }

        public string Name { get; set; }

        public Image Image { get; set; }

        public string Description { get; set; }

        public string SelectedItemId { get; set; }

        public int MinQuantity { get; set; }

        public int MaxQuantity { get; set; }

        public bool IsRequired { get; set; }

        public Product[] Items { get; set; }
    }
}
