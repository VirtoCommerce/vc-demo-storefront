using System.Collections.Generic;
using System.Threading.Tasks;
using PagedList.Core;

namespace VirtoCommerce.Storefront.Model.Cart.Services
{
    public interface ICartService
    {
        Task<IPagedList<CustomerOrder>> SearchCartsAsync(CartSearchCriteria criteria);
        Task<CustomerOrder> SaveChanges(CustomerOrder cart);
        Task<CustomerOrder> GetByIdAsync(string cartId);
        Task DeleteCartByIdAsync(string cartId);

        Task<IEnumerable<ShippingMethod>> GetAvailableShippingMethodsAsync(CustomerOrder cart);
        Task<IEnumerable<PaymentMethod>> GetAvailablePaymentMethodsAsync(CustomerOrder cart);

    }
}
