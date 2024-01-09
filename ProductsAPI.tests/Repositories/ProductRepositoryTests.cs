using Moq;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

using ProductsAPI.Entities;
using ProductsAPI.Repositories;
using ProductsAPI.Helpers;
using ProductsAPI.Data;

namespace ProductsAPI.Tests.Repositories;

public class ProductRepositoryTests
{
    private static readonly string DATABASE_NAME = "test";
    private static readonly string DEFAULT_PRODUCT_NAME = "Apple";
    private static readonly float DEFAULT_PRODUCT_PRICE = 0.1f;
    private static readonly int INEXISTENT_ID = 0;
    private static readonly float INVALID_PRICE = 0.0f;
    private static readonly string NEW_PRODUCT_NAME = "Peach";
    private static readonly float NEW_PRODUCT_PRICE = 9.5f;
    private readonly DatabaseContext context;
    private readonly ProductRepository repository;

    public ProductRepositoryTests()
    {
        context = new DatabaseContext(DATABASE_NAME);
        repository = new ProductRepository(context);
    }

    [Fact]
    public void When_AddProduct_Then_ShouldReturnSuccess()
    {
        // * Arrange
        CleanDatabase();
        ResultOfEntity<Product> productWrapper = GetCreatedProductWrapper();

        // * Act
        Result result = repository.Add(productWrapper.Entity);

        // * Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void When_AddProductWithExistentName_Then_ShouldReturnFailure()
    {
        // * Arrange
        CleanDatabase();
        ResultOfEntity<Product> productWrapper = GetCreatedProductWrapper();
        repository.Add(productWrapper.Entity);

        // * Act
        Result result = repository.Add(productWrapper.Entity);

        // * Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(ErrorMessages.NAME_ALREADY_EXISTS);
    }

    [Fact]
    public void When_DeleteProduct_Then_ShouldReturnSuccess()
    {
        // * Arrange
        CleanDatabase();
        int id = AddProductAndGetId();

        // * Act
        Result result = repository.DeleteById(id);

        // * Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void When_DeleteInexistentProduct_Then_ShouldReturnFailure()
    {
        // * Arrange
        CleanDatabase();
        int id = INEXISTENT_ID;

        // * Act
        Result result = repository.DeleteById(id);

        // * Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(ErrorMessages.PRODUCT_ID_NOT_FOUND(id));
    }

    [Fact]
    public void When_GetProductById_Then_ShouldReturnProduct()
    {
        // * Arrange
        CleanDatabase();
        int id = AddProductAndGetId();

        // * Act
        ResultOfEntity<Product> result = repository.GetById(id);

        // * Assert
        result.IsSuccess.Should().BeTrue();
        result.Entity.Should().NotBeNull();
    }

    [Fact]
    public void When_GetInexistentProductById_Then_ShouldReturnFailure()
    {
        // * Arrange
        CleanDatabase();
        int id = INEXISTENT_ID;

        // * Act
        ResultOfEntity<Product> result = repository.GetById(id);

        // * Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(ErrorMessages.PRODUCT_ID_NOT_FOUND(id)
        );
    }

    [Fact]
    public void When_UpdateProductWithNoInfo_Then_ShouldReturnSuccess()
    {
        // * Arrange
        CleanDatabase();
        int id = AddProductAndGetId();

        // * Act
        Result result = repository.Update(id: id, name: null, price: null);

        // * Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void When_UpdateProductWithName_Then_ShouldReturnSuccess()
    {
        // * Arrange
        CleanDatabase();
        int id = AddProductAndGetId();

        // * Act
        Result result = repository.Update(id: id, name: NEW_PRODUCT_NAME, price: null);

        // * Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void When_UpdateProductWithPrice_Then_ShouldReturnSuccess()
    {
        // * Arrange
        CleanDatabase();
        int id = AddProductAndGetId();

        // * Act
        Result result = repository.Update(id: id, name: null, price: NEW_PRODUCT_PRICE);

        // * Assert
        result.IsSuccess.Should().BeTrue();
    }


    [Fact]
    public void When_UpdateProductWithInvalidPrice_Then_ShouldReturnFailure()
    {
        // * Arrange
        CleanDatabase();
        int id = AddProductAndGetId();

        // * Act
        Result result = repository.Update(id: id, name: null, price: INVALID_PRICE);

        // * Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(ErrorMessages.INVALID_PRICE);
    }

    [Fact]
    public void When_UpdateProductWithNameAndPrice_Then_ShouldReturnSuccess()
    {
        // * Arrange
        CleanDatabase();
        int id = AddProductAndGetId();

        // * Act
        Result result = repository.Update(id: id, name: NEW_PRODUCT_NAME, price: NEW_PRODUCT_PRICE);

        // * Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void When_UpdateProductWithNameAndInvalidPrice_Then_ShouldReturnFailure()
    {
        // * Arrange
        CleanDatabase();
        int id = AddProductAndGetId();

        // * Act
        Result result = repository.Update(id: id, name: NEW_PRODUCT_NAME, price: INVALID_PRICE);

        // * Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(ErrorMessages.INVALID_PRICE);
    }

    private void CleanDatabase()
    {
        context.products.RemoveRange(context.products);
        context.SaveChanges();
    }

    private int AddProductAndGetId()
    {
        AddProduct();
        IEnumerable<Product> products = repository.GetAll();
        Product product = products.First();
        int id = product.Id;
        return id;
    }

    private ResultOfEntity<Product> GetCreatedProductWrapper()
    {
        ResultOfEntity<Product> productWrapper = Product.Create(DEFAULT_PRODUCT_NAME, DEFAULT_PRODUCT_PRICE);
        return productWrapper;
    }

    private void AddProduct()
    {
        ResultOfEntity<Product> productWrapper = GetCreatedProductWrapper();
        repository.Add(productWrapper.Entity);
    }
}
