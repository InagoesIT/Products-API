using System.Text;
using System.Text.Json;
using static System.Net.Mime.MediaTypeNames;
using System.Runtime.Intrinsics.X86;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

using ProductsAPI.Entities;
using ProductsAPI.Data;
using ProductsAPI.Helpers;
using ProductsAPI.Controllers;

namespace ProductsAPI.tests.Controllers;

public class ProductControllerTests
{
    private static readonly string DEFAULT_PRODUCT_NAME = "Apple";
    private static readonly float DEFAULT_PRODUCT_PRICE = 0.1f;
    private readonly string BASE_URL = "api/v1/products";
    private HttpClient HttpClient { get; set; }

    public ProductControllerTests()
    {
        var application = new WebApplicationFactory<ProductsController>().WithWebHostBuilder(builder => { });
        HttpClient = application.CreateClient();
        CleanDatabase();
    }

    protected void CleanDatabase()
    {
        DatabaseContext databaseContext = new DatabaseContext();
        databaseContext.Products.RemoveRange(databaseContext.Products.ToList());
        databaseContext.SaveChanges();
    }

    [Fact]
    public async void When_CreatedProduct_Then_ShouldReturnProductInTheGetRequest()
    {
        // * Arrange
        CleanDatabase();
        CreateProductDto createProductDto = CreateSUT();

        // * Act
        StringContent productAsJson = new StringContent(
                            JsonSerializer.Serialize(createProductDto),
                            Encoding.UTF8,
                            Application.Json
                            );
        var createProductResponse =
            await HttpClient.PostAsync(BASE_URL, productAsJson);

        var getProductsResult = await HttpClient.GetAsync(BASE_URL);

        // * Assert
        createProductResponse.EnsureSuccessStatusCode();
        createProductResponse.StatusCode.Should().Be(System.Net.HttpStatusCode.Created);

        getProductsResult.EnsureSuccessStatusCode();
        getProductsResult.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);

        var productsString = await getProductsResult.Content.ReadAsStringAsync();
        var products = JsonSerializer.Deserialize<List<ProductDto>>(productsString);
        products.Should().NotBeNull();
        products.Should().HaveCount(1);
    }

    private CreateProductDto CreateSUT()
    {
        return new CreateProductDto
        {
            Name = DEFAULT_PRODUCT_NAME,
            Price = DEFAULT_PRODUCT_PRICE
        };
    }
}
