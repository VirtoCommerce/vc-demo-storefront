using VirtoCommerce.Storefront.Model.Common;

namespace VirtoCommerce.Storefront.Model.Catalog
{
    public class ProductTotal
    {
        public ProductTotal(Currency currency)
        {
            Total = new Money(currency);
            TotalWithTax = new Money(currency);
            SubTotal = new Money(currency);
            SubTotalWithTax = new Money(currency);
            DiscountTotal = new Money(currency);
            DiscountTotalWithTax = new Money(currency);
            TaxTotal = new Money(currency);
        }
        public Money Total { get; set; }
        public Money TotalWithTax { get; set; }

        public Money SubTotal { get; set; }

        public Money SubTotalWithTax { get; set; }

        public Money DiscountTotal { get; set; }

        public Money DiscountTotalWithTax { get; set; }

        public Money TaxTotal { get; set; }
    }
}
