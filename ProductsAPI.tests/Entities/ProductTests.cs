// * KNOWLEDGE *
// * Given When Then -> used in functional testing
// * or When... Then ... Should

// * classically 1 assert per unit test
// * but can have more if they refer to the same context

// * 99% of tests return void and get no params

using ProductsAPI.Entities;
using ProductsAPI.Helpers;
using ProductsAPI.tests.Helpers;

namespace ProductsAPI.Tests.Entities;

public class ProductTests
{
    [Fact]
    public void When_CreateProduct_Then_ShouldReturnProduct()
    {
        // * Arrange
        (string name, float price) sut = CreateSUT();

        // * Act
        ResultOfEntity<Product> result = CreateProductFrom(sut);

        // * Assert
        result.IsSuccess.Should().BeTrue();
        result.Entity.Should().NotBeNull();
        result.Entity.Id.Should().NotBeEmpty();
        result.Entity.Name.Should().Be(sut.name);
        result.Entity.Price.Should().Be(sut.price);
    }

    [Fact]
    public void When_CreateProductWithZeroPrice_Then_ShouldReturnFailure()
    {
        // * Arrange
        (string name, float price) sut = CreateSUT();
        sut.price = TestingConstants.ZERO_PRICE;

        // * Act
        ResultOfEntity<Product> result = CreateProductFrom(sut);

        // * Assert
        result.IsSuccess.Should().BeFalse();
        result.Entity.Should().BeNull();
        result.Error.Should().Be(ErrorMessages.INVALID_PRICE);
    }

    [Fact]
    public void When_CreateProductWithNegativePrice_Then_ShouldReturnFailure()
    {
        // * Arrange
        (string name, float price) sut = CreateSUT();
        sut.price = TestingConstants.NEGATIVE_PRICE;

        // * Act
        ResultOfEntity<Product> result = CreateProductFrom(sut);

        // * Assert
        result.IsSuccess.Should().BeFalse();
        result.Entity.Should().BeNull();
        result.Error.Should().Be(ErrorMessages.INVALID_PRICE);
    }

    [Fact]
    public void When_CreateProductWithEmptyName_Then_ShouldReturnFailure()
    {
        // * Arrange
        (string name, float price) sut = CreateSUT();
        sut.name = string.Empty;

        // * Act
        ResultOfEntity<Product> result = CreateProductFrom(sut);

        // * Assert
        result.IsSuccess.Should().BeFalse();
        result.Entity.Should().BeNull();
        result.Error.Should().Be(ErrorMessages.EMPTY_NAME);
    }

    [Fact]
    public void When_UpdateProductName_Then_ShouldChangeName()
    {
        // * Arrange
        (string name, float price) sut = CreateSUT();
        ResultOfEntity<Product> productWrapper = CreateProductFrom(sut);
        Product product = productWrapper.Entity;

        // * Act
        product.Name = TestingConstants.NEW_PRODUCT_NAME;

        // * Assert
        product.Name.Should().Be(TestingConstants.NEW_PRODUCT_NAME);
        product.Price.Should().Be(sut.price);
    }

    [Fact]
    public void When_UpdateProductWithPrice_Then_ShouldReturnSuccess()
    {
        // * Arrange
        (string name, float price) sut = CreateSUT();
        ResultOfEntity<Product> productWrapper = CreateProductFrom(sut);
        Product product = productWrapper.Entity;

        // * Act
        Result result = product.UpdatePrice(price: TestingConstants.NEW_PRODUCT_PRICE);

        // * Assert
        result.IsSuccess.Should().BeTrue();
        result.Error.Should().BeNull();
        product.Name.Should().Be(sut.name);
        product.Price.Should().Be(TestingConstants.NEW_PRODUCT_PRICE);
    }

    [Fact]
    public void When_UpdateProductWithInvalidPrice_Then_ShouldReturnFailure()
    {
        // * Arrange
        (string name, float price) sut = CreateSUT();
        ResultOfEntity<Product> productWrapper = CreateProductFrom(sut);
        Product product = productWrapper.Entity;

        // * Act
        Result result = product.UpdatePrice(price: TestingConstants.NEGATIVE_PRICE);

        // * Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(ErrorMessages.INVALID_PRICE);
        product.Name.Should().Be(sut.name);
        product.Price.Should().Be(sut.price);
    }

    private (string name, float price) CreateSUT()
    {
        (string name, float price) result = (
            TestingConstants.DEFAULT_PRODUCT_NAME,
            TestingConstants.DEFAULT_PRODUCT_PRICE
            );
        return result;
    }

    private static ResultOfEntity<Product> CreateProductFrom((string name, float price) sut)
    {
        return Product.Create(
                    name: sut.name,
                    price: sut.price
                );
    }
}