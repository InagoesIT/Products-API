using ProductsAPI.Entities;
using ProductsAPI.Repositories;
using ProductsAPI.Helpers;

namespace ProductsAPI.Tests.Repositories;

public class ProductRepositoryTests
{
    [Fact]
    public void When_AddProduct_Then_ShouldReturnSuccess()
    {
        // * Arrange
        (string name, float price) sut = CreateSUT();
        ResultOfEntity<Product> result = Product.Create(sut.name, sut.price);

        // * Act


    }

    private (string name, float price) CreateSUT()
    {
        (string name, float price) result = ("Apple", 0.1f);
        return result;
    }
}
