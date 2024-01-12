using System.Text;
using System.Text.Json;
using static System.Net.Mime.MediaTypeNames;
using System.Runtime.Intrinsics.X86;
using Microsoft.AspNetCore.Mvc.Testing;

using ProductsAPI.Entities;
using ProductsAPI.Data;
using ProductsAPI.Helpers;
using ProductsAPI.Controllers;

namespace ProductsAPI.tests.Controllers;

public class ProductControllerTests
{
    private static readonly string DEFAULT_PRODUCT_NAME = "Apple";
    private static readonly float DEFAULT_PRODUCT_PRICE = 0.1f;
    private static readonly string DATABASE_NAME = "test";
    private readonly string BASE_URL = "api/v1/products";
    private HttpClient HttpClient { get; set; }
    private DatabaseContext databaseContext { get; set; }

    public ProductControllerTests()
    {
        var application = new WebApplicationFactory<ProductsController>().WithWebHostBuilder(builder => { });
        HttpClient = application.CreateClient();
        databaseContext = new DatabaseContext(DATABASE_NAME);
        CleanDatabase();
    }

    protected void CleanDatabase()
    {
        databaseContext.products.RemoveRange(databaseContext.products.ToList());
        databaseContext.SaveChanges();
    }

    [Fact]
    public async void When_CreatedProduct_Then_ShouldReturnProductInTheGetRequest()
    {
        // * Arrange
        ResultOfEntity<Product> productWrapper = Product.Create(DEFAULT_PRODUCT_NAME, DEFAULT_PRODUCT_PRICE);
        Product product = productWrapper.Entity;

        // * Act
        StringContent productAsJson = new StringContent(
                            JsonSerializer.Serialize(product),
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
        var products = JsonSerializer.Deserialize<List<Product>>(productsString);
        products.Should().NotBeNull();
        products.Should().HaveCount(1);
        CleanDatabase();
    }
}
