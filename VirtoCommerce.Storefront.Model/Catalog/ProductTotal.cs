using VirtoCommerce.Storefront.Model.Common;

namespace VirtoCommerce.Storefront.Model.Catalog
{
    public class ProductTotal
    {
        public Money Total { get; set; }

        public Money SubTotal { get; set; }

        public Money SubTotalWithTax { get; set; }

        public Money DiscountTotal { get; set; }

        public Money DiscountTotalWithTax { get; set; }

        public Money TaxTotal { get; set; }
    }
}
