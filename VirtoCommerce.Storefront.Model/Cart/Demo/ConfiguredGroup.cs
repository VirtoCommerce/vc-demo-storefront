using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using VirtoCommerce.Storefront.Infrastructure.Swagger;
using VirtoCommerce.Storefront.Model.Catalog;
using VirtoCommerce.Storefront.Model.Common;

namespace VirtoCommerce.Storefront.Model.Cart.Demo
{
    [SwaggerSchemaId("CartConfiguredGroup")]
    public class ConfiguredGroup : Entity
    {
        public ConfiguredGroup(int quantity, Currency currency, string productId)
        {
            Id = Guid.NewGuid().ToString("N");
            ProductId = productId;
            Quantity = quantity;
            Currency = currency;

            ExtendedPrice = new Money(currency);
            ExtendedPriceWithTax = new Money(currency);
            TaxTotal = new Money(currency);

            ListPrice = new Money(currency);
            ListPriceWithTax = new Money(currency);
            SalePrice = new Money(currency);
            SalePriceWithTax = new Money(currency);
            PlacedPrice = new Money(currency);
            PlacedPriceWithTax = new Money(currency);

            Items = new List<LineItem>();
        }

        public string ProductId { get; set; }

        public string Name { get; set; }

        public string ImageUrl { get; set; }

        public Product Product { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public string CreatedBy { get; set; }

        public string ModifiedBy { get; set; }

        [JsonRequired]
        public virtual IList<LineItem> Items { get; set; }

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

        #endregion Pricing

        #region Taxation

        [JsonRequired]
        public Money TaxTotal { get; set; }

        #endregion Taxation

        public override object Clone()
        {
            var result = (ConfiguredGroup) base.Clone();

            result.ListPrice = ListPrice?.Clone() as Money;
            result.SalePrice = SalePrice?.Clone() as Money;
            result.ListPriceWithTax = ListPriceWithTax?.Clone() as Money;
            result.SalePriceWithTax = SalePriceWithTax?.Clone() as Money;
            result.PlacedPrice = PlacedPrice?.Clone() as Money;
            result.PlacedPriceWithTax = PlacedPriceWithTax?.Clone() as Money;
            result.ExtendedPrice = ExtendedPrice?.Clone() as Money;
            result.ExtendedPriceWithTax = ExtendedPriceWithTax?.Clone() as Money;
            result.TaxTotal = TaxTotal?.Clone() as Money;

            result.Items = new List<LineItem>();

            return result;
        }
    }
}
