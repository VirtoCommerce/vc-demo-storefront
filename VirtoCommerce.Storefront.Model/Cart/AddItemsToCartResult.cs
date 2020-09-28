using System;
using System.Collections.Generic;
using System.Text;

namespace VirtoCommerce.Storefront.Model.Cart
{
    public class AddItemsToCartResult
    {
        public bool IsSuccess { get; set; }

        public string ErrorCode { get; set; }
    }
}
