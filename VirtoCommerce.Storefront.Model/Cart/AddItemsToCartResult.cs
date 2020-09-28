using System;
using System.Collections.Generic;
using System.Text;

namespace VirtoCommerce.Storefront.Model.Cart
{
    public class AddItemsToCartResult
    {
        public AddItemsToCartResult()
        {
            ErrorCodes = new string[] { };
        }
        public bool IsSuccess { get; set; }

        public string[] ErrorCodes { get; set; }
    }
}
