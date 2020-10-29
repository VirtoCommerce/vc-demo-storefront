using System;
using System.Linq;
using VirtoCommerce.Storefront.Model.Order;
using VirtoCommerce.Storefront.Model.Order.Demo;
using VirtoCommerce.Storefront.Model.Common;
using orderDto = VirtoCommerce.Storefront.AutoRestClients.OrdersModuleApi.Models;

namespace VirtoCommerce.Storefront.Domain
{
    public static partial class OrderConverter
    {
        public static orderDto.DemoOrderConfiguredGroup ToConfiguredGroup(this ConfiguredGroup group)
        {
            foreach (var lineItem in group.Items)
            {
                lineItem.Id = lineItem.Id ?? Guid.NewGuid().ToString("N");
            }

            return new orderDto.DemoOrderConfiguredGroup
            {
                Id = group.Id ?? Guid.NewGuid().ToString("N"),
                ProductId = group.ProductId,
                ItemIds = group.Items.Select(x => x.Id).ToList(),
                CreatedBy = group.CreatedBy,
                CreatedDate = group.CreatedDate,
                ModifiedBy = group.ModifiedBy,
                ModifiedDate = group.ModifiedDate,
                Currency = group.Currency.Code,
                ExtendedPrice = (double)group.ExtendedPrice.InternalAmount,
                ExtendedPriceWithTax = (double)group.ExtendedPriceWithTax.InternalAmount,
                Price = (double)group.Price.InternalAmount,
                PriceWithTax = (double)group.PriceWithTax.InternalAmount,
                PlacedPrice = (double)group.PlacedPrice.InternalAmount,
                PlacedPriceWithTax = (double)group.PlacedPriceWithTax.InternalAmount,
                TaxTotal = (double)group.TaxTotal.InternalAmount,
                Quantity = group.Quantity
            };
        }

        public static ConfiguredGroup ToConfiguredGroup(this orderDto.DemoOrderConfiguredGroup group, CustomerOrder order)
        {
            var result = new ConfiguredGroup(group.Quantity ?? 0, order.Currency, group.ProductId)
            {
                Id = group.Id,                
                CreatedBy = group.CreatedBy,
                CreatedDate = group.CreatedDate ?? DateTime.UtcNow,
                ModifiedBy = group.ModifiedBy,
                ModifiedDate = group.ModifiedDate,

                ExtendedPrice = new Money(group.ExtendedPrice ?? 0, order.Currency),
                ExtendedPriceWithTax = new Money(group.ExtendedPriceWithTax ?? 0, order.Currency),
                TaxTotal = new Money(group.TaxTotal ?? 0, order.Currency),

                Price = new Money(group.Price ?? 0, order.Currency),
                PriceWithTax = new Money(group.PriceWithTax ?? 0, order.Currency),
                PlacedPrice = new Money(group.PlacedPrice ?? 0, order.Currency),
                PlacedPriceWithTax = new Money(group.PlacedPriceWithTax ?? 0, order.Currency),
                Currency = order.Currency,
            };

            foreach (var item in group.ItemIds.Select(id => order.Items.First(x => x.Id == id)))
            {
                result.Items.Add(item);
            }

            return result;
        }
    }
}
