using ProductsAPI.Entities;
using ProductsAPI.Repositories;
using ProductsAPI.Helpers;
using ProductsAPI.Data;
using ProductsAPI.tests.Helpers;

namespace ProductsAPI.Tests.Repositories;

public class ProductRepositoryTests
{
    private static readonly string DATABASE_NAME = "testing";
    private readonly DatabaseContext DatabaseContext;
    private readonly ProductRepository Repository;

    public ProductRepositoryTests()
    {
        DatabaseContext = new DatabaseContext(DATABASE_NAME);
        Repository = new ProductRepository(DatabaseContext);
    }

    [Fact]
    public void When_AddProduct_Then_ShouldReturnSuccess()
    {
        // * Arrange
        CleanDatabase();
        ResultOfEntity<Product> productWrapper = GetCreatedProductWrapper();

        // * Act
        Result result = Repository.Add(productWrapper.Entity);

        // * Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void When_AddProductWithExistentName_Then_ShouldReturnFailure()
    {
        // * Arrange
        CleanDatabase();
        ResultOfEntity<Product> productWrapper = GetCreatedProductWrapper();
        Repository.Add(productWrapper.Entity);

        // * Act
        Result result = Repository.Add(productWrapper.Entity);

        // * Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(ErrorMessages.NAME_ALREADY_EXISTS);
    }

    [Fact]
    public void When_DeleteProduct_Then_ShouldReturnSuccess()
    {
        // * Arrange
        CleanDatabase();
        Guid id = AddProductAndGetId();

        // * Act
        Result result = Repository.DeleteById(id);

        // * Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void When_DeleteInexistentProduct_Then_ShouldReturnFailure()
    {
        // * Arrange
        CleanDatabase();
        Guid id = TestingConstants.INEXISTENT_ID;

        // * Act
        Result result = Repository.DeleteById(id);

        // * Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(ErrorMessages.PRODUCT_ID_NOT_FOUND(id));
    }

    [Fact]
    public void When_GetProductById_Then_ShouldReturnProduct()
    {
        // * Arrange
        CleanDatabase();
        Guid id = AddProductAndGetId();

        // * Act
        ResultOfEntity<Product> result = Repository.GetById(id);

        // * Assert
        result.IsSuccess.Should().BeTrue();
        result.Entity.Should().NotBeNull();
    }

    [Fact]
    public void When_GetInexistentProductById_Then_ShouldReturnFailure()
    {
        // * Arrange
        CleanDatabase();
        Guid id = TestingConstants.INEXISTENT_ID;

        // * Act
        ResultOfEntity<Product> result = Repository.GetById(id);

        // * Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(ErrorMessages.PRODUCT_ID_NOT_FOUND(id)
        );
    }

    [Fact]
    public void When_GetProductByName_Then_ShouldReturnProduct()
    {
        // * Arrange
        CleanDatabase();
        ResultOfEntity<Product> productWrapper = GetCreatedProductWrapper();
        Product product = productWrapper.Entity;
        Repository.Add(product);
        string name = product.Name;

        // * Act
        ResultOfEntity<Product> result = Repository.GetByName(name);

        // * Assert
        result.IsSuccess.Should().BeTrue();
        result.Entity.Should().NotBeNull();
    }

    [Fact]
    public void When_GetInexistentProductByName_Then_ShouldReturnFailure()
    {
        // * Arrange
        CleanDatabase();
        string name = TestingConstants.INEXISTENT_NAME;

        // * Act
        ResultOfEntity<Product> result = Repository.GetByName(name);

        // * Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(ErrorMessages.PRODUCT_NAME_NOT_FOUND(name)
        );
    }

    [Fact]
    public void When_UpdateProductWithNoInfo_Then_ShouldReturnSuccess()
    {
        // * Arrange
        CleanDatabase();
        Guid id = AddProductAndGetId();

        // * Act
        Result result = Repository.Update(id: id, name: null, price: null);

        // * Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void When_UpdateProductWithName_Then_ShouldReturnSuccess()
    {
        // * Arrange
        CleanDatabase();
        Guid id = AddProductAndGetId();

        // * Act
        Result result = Repository.Update(id: id, name: TestingConstants.NEW_PRODUCT_NAME, price: null);

        // * Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void When_UpdateProductWithPrice_Then_ShouldReturnSuccess()
    {
        // * Arrange
        CleanDatabase();
        Guid id = AddProductAndGetId();

        // * Act
        Result result = Repository.Update(id: id, name: null, price: TestingConstants.NEW_PRODUCT_PRICE);

        // * Assert
        result.IsSuccess.Should().BeTrue();
    }


    [Fact]
    public void When_UpdateProductWithInvalidPrice_Then_ShouldReturnFailure()
    {
        // * Arrange
        CleanDatabase();
        Guid id = AddProductAndGetId();

        // * Act
        Result result = Repository.Update(id: id, name: null, price: TestingConstants.INVALID_PRICE);

        // * Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(ErrorMessages.INVALID_PRICE);
    }

    [Fact]
    public void When_UpdateProductWithNameAndPrice_Then_ShouldReturnSuccess()
    {
        // * Arrange
        CleanDatabase();
        Guid id = AddProductAndGetId();

        // * Act
        Result result = Repository.Update(id: id, name: TestingConstants.NEW_PRODUCT_NAME, price: TestingConstants.NEW_PRODUCT_PRICE);

        // * Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void When_UpdateProductWithNameAndInvalidPrice_Then_ShouldReturnFailure()
    {
        // * Arrange
        CleanDatabase();
        Guid id = AddProductAndGetId();

        // * Act
        Result result = Repository.Update(id: id, name: TestingConstants.NEW_PRODUCT_NAME, price: TestingConstants.INVALID_PRICE);

        // * Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(ErrorMessages.INVALID_PRICE);
    }

    private void CleanDatabase()
    {
        DatabaseContext.Products.RemoveRange(DatabaseContext.Products);
        DatabaseContext.SaveChanges();
    }

    private Guid AddProductAndGetId()
    {
        AddProduct();
        IEnumerable<Product> products = Repository.GetAll();
        Product product = products.First();
        Guid id = product.Id;
        return id;
    }

    private void AddProduct()
    {
        ResultOfEntity<Product> productWrapper = GetCreatedProductWrapper();
        Repository.Add(productWrapper.Entity);
    }

    private ResultOfEntity<Product> GetCreatedProductWrapper()
    {
        ResultOfEntity<Product> productWrapper = Product.Create(
            TestingConstants.DEFAULT_PRODUCT_NAME,
            TestingConstants.DEFAULT_PRODUCT_PRICE
            );
        return productWrapper;
    }
}
