using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using VirtoCommerce.Storefront.Model.Catalog;
using VirtoCommerce.Storefront.Model.Common;

namespace VirtoCommerce.Storefront.Model.Cart.Demo
{
    public class ConfiguredGroup : Entity
    {
        public ConfiguredGroup(int quantity, Currency currency, Money extendedPrice,
            Money extendedPriceWithTax, Money taxTotal)
        {
            Quantity = quantity;
            Currency = currency;
            ExtendedPrice = extendedPrice;
            ExtendedPriceWithTax = extendedPriceWithTax;
            TaxTotal = taxTotal;            
        }


        public DateTime CreatedDate { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public string CreatedBy { get; set; }

        public string ModifiedBy { get; set; }

        [JsonRequired]
        public virtual IList<LineItem> Items { get; set; } = new List<LineItem>();

        [JsonRequired]
        public int Quantity { get; set; }

        #region Pricing

        [JsonRequired]
        public Currency Currency { get; set; }

        public Money ListPrice { get; set; }

        public Money ListPriceWithTax { get; set; }

        public Money SalePrice { get; set; }

        public Money SalePriceWithTax { get; set; }

        public Money PlacedPrice { get; set; }

        public Money PlacedPriceWithTax { get; set; }

        [JsonRequired]
        public Money ExtendedPrice { get; set; }

        [JsonRequired]
        public Money ExtendedPriceWithTax { get; set; }

        #endregion

        #region Taxation

        [JsonRequired]
        public Money TaxTotal { get; set; }

        #endregion

        public ICollection<ProductPart> Parts { get; set; } = new List<ProductPart>();
    }
}
