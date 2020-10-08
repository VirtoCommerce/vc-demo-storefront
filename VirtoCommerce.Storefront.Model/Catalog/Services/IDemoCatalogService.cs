using System.Threading.Tasks;
using VirtoCommerce.Storefront.Model.Services;

namespace VirtoCommerce.Storefront.Model.Catalog.Services
{
    public interface IDemoCatalogService: ICatalogService
    {
        Task<ProductPart[]> GetProductPartsAsync(string productId);

        ProductPart TryGetProductPartByCategoryId(string categoryId);
    }
}
