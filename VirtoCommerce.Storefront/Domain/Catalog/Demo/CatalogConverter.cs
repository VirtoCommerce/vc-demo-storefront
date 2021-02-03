using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Catalog;
using VirtoCommerce.Storefront.Model.Common;
using demoCatalogDto = VirtoCommerce.Storefront.AutoRestClients.DemoSolutionFeaturesModuleModuleApi.Models;


namespace VirtoCommerce.Storefront.Domain
{
    public static partial class CatalogConverter
    {
        public static ProductPart ToProductPart(this demoCatalogDto.DemoProductPart productPartDto, string currentLanguage)
        {
            var result = new ProductPart()
            {
                Id = productPartDto.Id,
                Name = productPartDto.Name,
                Description = productPartDto.Description,
                Image = new Image { Url = productPartDto.ImgSrc.RemoveLeadingUriScheme(), LanguageCode = currentLanguage },
                IsRequired = productPartDto.IsRequired ?? true,
                MinQuantity = productPartDto.MinQuantity ?? 0,
                MaxQuantity = productPartDto.MaxQuantity ?? 0,
                SelectedItemId = productPartDto.DefaultItemId,
            };

            return result;
        }
    }
}
