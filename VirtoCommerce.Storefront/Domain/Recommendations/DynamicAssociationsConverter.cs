using System.Linq;
using VirtoCommerce.Storefront.Model.Recommendations;
using dto = VirtoCommerce.Storefront.AutoRestClients.DynamicAssociationsModuleModuleApi.Models;

namespace VirtoCommerce.Storefront.Domain
{
    public static class DynamicAssociationsConverterExtension
    {
        public static DynamicAssociationsConverter DynamicAssociationsConverterInstance
        {
            get
            {
                return new DynamicAssociationsConverter();
            }
        }
        public static dto.AssociationEvaluationContext ToContextDto(this DynamicAssociationsEvalContext context)
        {
            return DynamicAssociationsConverterInstance.ToContextDto(context);
        }
    }

    public partial class DynamicAssociationsConverter
    {
        public virtual dto.AssociationEvaluationContext ToContextDto(DynamicAssociationsEvalContext context)
        {
            var retVal = new dto.AssociationEvaluationContext
            {
                StoreId = context.StoreId,
                ProductsToMatch = context.ProductIds.Where(x => !string.IsNullOrEmpty(x)).ToList(),
                Group = context.Group,
                Take = context.Take,
                Skip = context.Skip
            };

            return retVal;
        }
    }
}
