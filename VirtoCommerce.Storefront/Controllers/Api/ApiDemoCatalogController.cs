using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using VirtoCommerce.Storefront.Infrastructure;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Catalog;
using VirtoCommerce.Storefront.Model.Catalog.Services;
using VirtoCommerce.Storefront.Model.Common;

namespace VirtoCommerce.Storefront.Controllers.Api
{
    [StorefrontApiRoute("demo")]
    [ResponseCache(CacheProfileName = "None")]
    public class ApiDemoCatalogController: StorefrontControllerBase
    {
        private readonly IDemoCatalogService _demoCatalogService;

        public ApiDemoCatalogController(IWorkContextAccessor workContextAccessor, IStorefrontUrlBuilder urlBuilder, IDemoCatalogService demoCatalogService) : base(workContextAccessor, urlBuilder)
        {
            _demoCatalogService = demoCatalogService;
        }

        [HttpGet("catalog/{productId}/configuration")]
        public async Task<ActionResult<ProductPart[]>> GetProductConfiguration(string productId)
        {
            var result = await _demoCatalogService.GetProductPartsAsync(productId);

            foreach (var product in result.SelectMany(x => x.Items))
            {
                product.Url = UrlBuilder.ToAppAbsolute(product.Url);
            }

            return result;
        }
    }
}
