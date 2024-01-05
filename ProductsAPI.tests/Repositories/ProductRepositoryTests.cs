using Moq;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

using ProductsAPI.Entities;
using ProductsAPI.Repositories;
using ProductsAPI.Helpers;
using ProductsAPI.Data;

namespace ProductsAPI.Tests.Repositories;

// * mocking inspiration: https://learn.microsoft.com/en-us/ef/ef6/fundamentals/testing/mocking

public class ProductRepositoryTests
{
    [Fact]
    public void When_AddProduct_Then_ShouldReturnSuccess()
    {
        // * Arrange
        DatabaseContext context = new DatabaseContext("test");
        context.products.RemoveRange(context.products);
        context.SaveChanges();

        (string name, float price) sut = CreateProductSUT();
        ResultOfEntity<Product> productWrapper = Product.Create(sut.name, sut.price);

        ProductRepository repository = new ProductRepository(context);

        // * Act
        Result result = repository.Add(productWrapper.Entity);

        // * Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void When_AddProductWithExistentName_Then_ShouldReturnFailure()
    {
        // * Arrange
        DatabaseContext context = new DatabaseContext("test");
        context.products.RemoveRange(context.products);
        context.SaveChanges();

        (string name, float price) sut = CreateProductSUT();
        ResultOfEntity<Product> productWrapper = Product.Create(sut.name, sut.price);

        ProductRepository repository = new ProductRepository(context);
        repository.Add(productWrapper.Entity);

        // * Act
        Result result = repository.Add(productWrapper.Entity);

        // * Assert
        result.IsSuccess.Should().BeFalse();
    }

    private (string name, float price) CreateProductSUT()
    {
        (string name, float price) result = ("Apple", 0.1f);
        return result;
    }


}
