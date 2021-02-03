using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using VirtoCommerce.Storefront.Infrastructure.Swagger;
using VirtoCommerce.Storefront.Model.Catalog;
using VirtoCommerce.Storefront.Model.Common;

namespace VirtoCommerce.Storefront.Model.Order.Demo
{
    [SwaggerSchemaId("OrderConfiguredGroup")]
    public class ConfiguredGroup : Entity
    {
        public ConfiguredGroup(int quantity, Currency currency, string productId)
        {
            Id = Guid.NewGuid().ToString("N");
            Items = new List<LineItem>();
            ProductId = productId;
            Quantity = quantity;
            Currency = currency;

            ExtendedPrice = new Money(currency);
            ExtendedPriceWithTax = new Money(currency);
            TaxTotal = new Money(currency);

            Price = new Money(currency);
            PriceWithTax = new Money(currency);
            PlacedPrice = new Money(currency);
            PlacedPriceWithTax = new Money(currency);
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

        public Money Price { get; set; }

        public Money PriceWithTax { get; set; }

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
    }
}
