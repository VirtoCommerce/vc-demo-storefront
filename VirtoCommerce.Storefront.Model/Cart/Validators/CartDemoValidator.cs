using FluentValidation;

namespace VirtoCommerce.Storefront.Model.Cart.Validators
{
    public class CartDemoValidator : AbstractValidator<ShoppingCart>
    {
        public CartDemoValidator()
        {
            RuleFor(x => x.Name).NotNull().NotEmpty();
            RuleFor(x => x.Currency).NotNull();
            RuleFor(x => x.CustomerId).NotNull().NotEmpty();

            RuleSet("strict", () =>
            {
                RuleForEach(x => x.Items).SetValidator(cart => new CartLineItemDemoValidator(cart));               
            });

        }
    }
}
