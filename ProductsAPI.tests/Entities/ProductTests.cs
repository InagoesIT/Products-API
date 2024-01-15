using ProductsAPI.Entities;
using ProductsAPI.Helpers;

namespace ProductsAPI.Tests.Entities;

public class ProductTests
{
    // * KNOWLEDGE *
    // * Given When Then -> used in functional testing
    // * or When... Then ... Should

    // * maximum 1 assert per unit test
    // * but can have more if they refer to the same context

    // * 99% of tests return void and get no params

    [Fact]
    public void When_CreateProduct_Then_ShouldReturnProduct()
    {
        // * Arrange
        (string name, float price) sut = CreateSUT();

        // * Act
        ResultOfEntity<Product> result = Product.Create(
            name: sut.name,
            price: sut.price
        );

        // * Assert
        result.IsSuccess.Should().BeTrue();
        result.Entity.Should().NotBeNull();
        result.Entity.Id.Should().BeGreaterThanOrEqualTo(0);
        result.Entity.Name.Should().Be(sut.name);
        result.Entity.Price.Should().Be(sut.price);
    }

    [Fact]
    public void When_CreateProductWithZeroPrice_Then_ShouldReturnFailure()
    {
        // * Arrange
        (string name, float price) sut = CreateSUT();
        sut.price = 0;

        // * Act
        ResultOfEntity<Product> result = Product.Create(
            name: sut.name,
            price: sut.price
        );

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
        sut.price = -1;

        // * Act
        ResultOfEntity<Product> result = Product.Create(
            name: sut.name,
            price: sut.price
        );

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
        sut.name = "";

        // * Act
        ResultOfEntity<Product> result = Product.Create(
            name: sut.name,
            price: sut.price
        );

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
        ResultOfEntity<Product> productWrapper = Product.Create(
            name: sut.name,
            price: sut.price
        );
        Product product = productWrapper.Entity;
        string newName = "Berry";

        // * Act
        product.Name = newName;

        // * Assert
        product.Name.Should().Be(newName);
        product.Price.Should().Be(sut.price);
    }

    [Fact]
    public void When_UpdateProductWithPrice_Then_ShouldReturnSuccess()
    {
        // * Arrange
        (string name, float price) sut = CreateSUT();
        ResultOfEntity<Product> productWrapper = Product.Create(
            name: sut.name,
            price: sut.price
        );
        Product product = productWrapper.Entity;
        float newPrice = 55.56f;

        // * Act
        Result result = product.UpdatePrice(price: newPrice);

        // * Assert
        result.IsSuccess.Should().BeTrue();
        result.Error.Should().BeNull();
        product.Name.Should().Be(sut.name);
        product.Price.Should().Be(newPrice);
    }

    [Fact]
    public void When_UpdateProductWithInvalidPrice_Then_ShouldReturnFailure()
    {
        // * Arrange
        (string name, float price) sut = CreateSUT();
        ResultOfEntity<Product> productWrapper = Product.Create(
            name: sut.name,
            price: sut.price
        );
        Product product = productWrapper.Entity;
        float newPrice = -8;

        // * Act
        Result result = product.UpdatePrice(price: newPrice);

        // * Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(ErrorMessages.INVALID_PRICE);
        product.Name.Should().Be(sut.name);
        product.Price.Should().Be(sut.price);
    }

    private (string name, float price) CreateSUT()
    {
        (string name, float price) result = ("Apple", 0.1f);
        return result;
    }
}