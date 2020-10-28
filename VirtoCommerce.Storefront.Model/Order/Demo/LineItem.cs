using VirtoCommerce.Storefront.Model.Catalog;

namespace VirtoCommerce.Storefront.Model.Order
{
    public partial class LineItem
    {
        public string ConfiguredGropupId { get; set; }

        public Product Product { get; set; }
    }
}
