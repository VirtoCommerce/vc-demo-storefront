using VirtoCommerce.Storefront.Model.Catalog;

namespace VirtoCommerce.Storefront.Model
{
    /// <summary>
    /// Represent menu link  associated to category 
    /// </summary>
    public class CategoryMenuLink : MenuLink
    {

        public Category Category { get; set; }
        public override string Type => "collection";
    }
}
