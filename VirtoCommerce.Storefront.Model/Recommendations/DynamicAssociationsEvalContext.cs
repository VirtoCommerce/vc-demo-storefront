namespace VirtoCommerce.Storefront.Model.Recommendations
{
    public partial class DynamicAssociationsEvalContext : RecommendationEvalContext
    {
        public string Group { get; set; } = "Accessories";

        public int Skip { get; set; } = 0;
    }
}
