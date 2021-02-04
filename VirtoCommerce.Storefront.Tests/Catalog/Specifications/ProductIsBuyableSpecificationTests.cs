using System;
using Xunit;
using Bogus;
using VirtoCommerce.Storefront.Model.Catalog;
using VirtoCommerce.Storefront.Model.Catalog.Demo;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model;

namespace VirtoCommerce.Storefront.Tests.Catalog.Specifications
{
    public class ProductIsBuyableSpecificationTests
    {
        const string CURRENCY_CODE = "USD";
        static readonly Currency Usd = new Currency(Language.InvariantLanguage, CURRENCY_CODE);
        static readonly Randomizer Rand = new Randomizer();
        static readonly Faker Faker = new Faker();

        [Fact]
        public void IsSatisfiedBy_NotConfigurable_True()
        {
            // arrange
            var product = new Product()
            {
                ProductType = ProductTypes.Physical,
                IsActive = true,
                IsBuyable = true,
                Price = new ProductPrice(Usd)
                {
                    ListPrice = new Money(Rand.Decimal(1), Usd)
                }
            };

            var spec = new ProductIsBuyableSpecification();

            // act
            var result = spec.IsSatisfiedBy(product);

            // assert
            Assert.True(result);
        }

        [Theory]
        [InlineData(true, false, 1, int.MaxValue)]
        [InlineData(false, true, 1, int.MaxValue)]
        [InlineData(false, false, 1, int.MaxValue)]
        [InlineData(false, false, 0, 0)]
        [InlineData(true, true, 0, 0)]
        public void IsSatisfiedBy_NotConfigurable_False(bool isActive, bool isBuyable, int priceMin, int priceMax)
        {
            // arrange
            var product = new Product()
            {
                ProductType = ProductTypes.Physical,
                IsActive = isActive,
                IsBuyable = isBuyable,
                Price = new ProductPrice(Usd)
                {
                    ListPrice = new Money(Rand.Decimal(priceMin, priceMax), Usd)
                }
            };

            var spec = new ProductIsBuyableSpecification();

            // act
            var result = spec.IsSatisfiedBy(product);

            // assert
            Assert.False(result);
        }

        [Fact]
        public void IsSatisfiedBy_Configurable_True()
        {
            // arrange
            var partItemFaker = new Faker<Product>()
                 .RuleFor(p => p.ProductType, f => ProductTypes.Physical);

            var partFaker = new Faker<ProductPart>()
                .RuleFor(p => p.Items, f => partItemFaker.Generate(Rand.Int(1, 5)).ToArray());

            var productFaker = new Faker<Product>()
                .RuleFor(p => p.ProductType, f => ProductTypes.Configurable)
                .RuleFor(p => p.Parts, f => partFaker.Generate(Rand.Int(1, 5)).ToArray());

            var product = productFaker.Generate();

            var spec = new ProductIsBuyableSpecification();

            // act
            var result = spec.IsSatisfiedBy(product);

            // assert
            Assert.True(result);
        }

        [Fact]        
        public void IsSatisfiedBy_ConfigurablePartsIsEmpty_False()
        {
            // arrange
            var partItemFaker = new Faker<Product>()
                 .RuleFor(p => p.ProductType, f => ProductTypes.Physical);

            var partFaker = new Faker<ProductPart>()
                .RuleFor(p => p.Items, f => partItemFaker.Generate(Rand.Int(1, 5)).ToArray());

            var productFaker = new Faker<Product>()
                .RuleFor(p => p.ProductType, f => ProductTypes.Configurable)
                .RuleFor(p => p.Parts, f => partFaker.Generate(Rand.Int(1, 5)).ToArray());            

            var product = productFaker.Generate();

            product.Parts = Array.Empty<ProductPart>();

            var spec = new ProductIsBuyableSpecification();

            // act
            var result = spec.IsSatisfiedBy(product);

            // assert
            Assert.False(result);
        }

        [Fact]
        public void IsSatisfiedBy_ConfigurableAnyPartItemsIsEmpty_False()
        {
            // arrange
            var partItemFaker = new Faker<Product>()
                 .RuleFor(p => p.ProductType, f => ProductTypes.Physical);

            var partFaker = new Faker<ProductPart>()
                .RuleFor(p => p.Items, f => partItemFaker.Generate(Rand.Int(1, 5)).ToArray());

            var productFaker = new Faker<Product>()
                .RuleFor(p => p.ProductType, f => ProductTypes.Configurable)
                .RuleFor(p => p.Parts, f => partFaker.Generate(Rand.Int(1, 5)).ToArray());

            var product = productFaker.Generate();

            var part = Faker.PickRandom(product.Parts);

            part.Items= Array.Empty<Product>();

            var spec = new ProductIsBuyableSpecification();

            // act
            var result = spec.IsSatisfiedBy(product);

            // assert
            Assert.False(result);
        }
    }
}

