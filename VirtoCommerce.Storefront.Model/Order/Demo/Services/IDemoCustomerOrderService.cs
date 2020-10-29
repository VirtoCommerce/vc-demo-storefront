using System.Threading.Tasks;

namespace VirtoCommerce.Storefront.Model.Order.Services
{
    public interface IDemoCustomerOrderService
    {
        Task LoadProductsAsync(params CustomerOrder[] orders);

        void SelectConfiguredProductParts(params CustomerOrder[] orders);
    }
}
