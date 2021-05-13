namespace VirtoCommerce.Storefront.Model.Recommendations
{
    public partial class CognitiveRecommendationEvalContext : RecommendationEvalContext
    {
        /// <summary>
        /// Model (scope) identifier for requested associations (used for Cognitive provider only)
        /// </summary>
        public string ModelId { get; set; }
        /// <summary>
        /// Build identifier used for
        /// </summary>
        public string BuildId { get; set; }

    }
}
