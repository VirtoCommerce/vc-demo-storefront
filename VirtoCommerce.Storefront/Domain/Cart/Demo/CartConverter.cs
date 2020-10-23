using System;
using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.Storefront.Common;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Cart;
using VirtoCommerce.Storefront.Model.Cart.Demo;
using VirtoCommerce.Storefront.Model.Catalog;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.Marketing;
using VirtoCommerce.Storefront.Model.Security;
using VirtoCommerce.Storefront.Model.Stores;
using VirtoCommerce.Storefront.Model.Tax;
using cartDto = VirtoCommerce.Storefront.AutoRestClients.CartModuleApi.Models;


namespace VirtoCommerce.Storefront.Domain
{

    public static partial class CartConverter
    {
        public static cartDto.DemoCartConfiguredGroup ToConfiguredGroup(this ConfiguredGroup group)
        {
            foreach (var lineItem in group.Items)
            {
                lineItem.Id = lineItem.Id ?? Guid.NewGuid().ToString("N");
            }

            return new cartDto.DemoCartConfiguredGroup
            {
                Id = group.Id ?? Guid.NewGuid().ToString("N"),
                ItemIds = group.Items.Select(x => x.Id).ToList(),                
                CreatedBy = group.CreatedBy,
                CreatedDate = group.CreatedDate,
                ModifiedBy = group.ModifiedBy,
                ModifiedDate = group.ModifiedDate,
                Currency = group.Currency.Code,
                ExtendedPrice = (double)group.ExtendedPrice.InternalAmount,
                ExtendedPriceWithTax = (double)group.ExtendedPriceWithTax.InternalAmount,
                ListPrice = (double)group.ListPrice.InternalAmount,
                ListPriceWithTax = (double)group.ListPriceWithTax.InternalAmount,
                PlacedPrice = (double)group.PlacedPrice.InternalAmount,
                PlacedPriceWithTax = (double)group.PlacedPriceWithTax.InternalAmount,
                SalePrice = (double)group.SalePrice.InternalAmount,
                SalePriceWithTax = (double)group.SalePriceWithTax.InternalAmount,
                TaxTotal = (double)group.TaxTotal.InternalAmount,                
                Quantity = group.Quantity
            };
        }

        public static ConfiguredGroup ToConfiguredGroup(this cartDto.DemoCartConfiguredGroup group, ShoppingCart cart)
        {
            var result = new ConfiguredGroup(
                group.Quantity ?? 0, cart.Currency, new Money(group.ExtendedPrice ?? 0, cart.Currency),
                new Money(group.ExtendedPriceWithTax ?? 0, cart.Currency),
                new Money(group.TaxTotal ?? 0, cart.Currency))
            {
                Id = group.Id,
                CreatedBy = group.CreatedBy,
                CreatedDate = group.CreatedDate ?? DateTime.UtcNow,
                ModifiedBy = group.ModifiedBy,
                ModifiedDate = group.ModifiedDate,
                ListPrice = new Money(group.ListPrice ?? 0, cart.Currency),
                ListPriceWithTax = new Money(group.ListPriceWithTax ?? 0, cart.Currency),
                SalePrice = new Money(group.SalePrice ?? 0, cart.Currency),
                SalePriceWithTax = new Money(group.SalePriceWithTax ?? 0, cart.Currency),
                PlacedPrice = new Money(group.PlacedPrice ?? 0, cart.Currency),
                PlacedPriceWithTax = new Money(group.PlacedPriceWithTax ?? 0, cart.Currency),
                Currency = cart.Currency,
                
            };

            foreach (var item in group.ItemIds.Select(id => cart.Items.First(x => x.Id == id)))
            {
                result.Items.Add(item);
            }

            return result;
        }
    }
}
