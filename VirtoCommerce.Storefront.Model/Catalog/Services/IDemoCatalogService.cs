using System.Threading.Tasks;
using VirtoCommerce.Storefront.Model.Catalog;

namespace VirtoCommerce.Storefront.Model.Services
{
    public interface IDemoCatalogService: ICatalogService
    {
        Task<ProductPart[]> GetProductPartsAsync(string productId);
    }
}
