using System;

namespace VirtoCommerce.Storefront.Model.Cart
{
    public class AddItemsToCartResult
    {
        public AddItemsToCartResult()
        {
            ErrorCodes = Array.Empty<string>();
        }

        public bool IsSuccess { get; set; }

        public string[] ErrorCodes { get; set; }
    }
}
