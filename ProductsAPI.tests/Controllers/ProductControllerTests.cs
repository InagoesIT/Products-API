using System.Text;
using System.Text.Json;
using static System.Net.Mime.MediaTypeNames;
using Microsoft.AspNetCore.Mvc.Testing;

using ProductsAPI.Entities;
using ProductsAPI.Data;
using ProductsAPI.Controllers;
using ProductsAPI.Helpers;

namespace ProductsAPI.tests.Controllers;

// TODO: test GET by id method
// TODO: test GET by name method
// TODO: test PUT method
// TODO: test DELETE method

public class ProductControllerTests
{
    private static readonly string DEFAULT_PRODUCT_NAME = "Apple";
    private static readonly float DEFAULT_PRODUCT_PRICE = 0.1f;
    private static readonly float INVALID_PRODUCT_PRICE = 0f;
    private static readonly string EMPTY_STRING = "";
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

    [Fact]
    public async void When_CreatedProductWithInvalidPrice_Then_ShouldReturnFailureAndNotBeReturnedInGet()
    {
        // * Arrange
        CleanDatabase();
        CreateProductDto createProductDto = CreateSUT();
        createProductDto.Price = INVALID_PRODUCT_PRICE;

        // * Act
        StringContent productAsJson = new StringContent(
                            JsonSerializer.Serialize(createProductDto),
                            Encoding.UTF8,
                            Application.Json
                            );
        var createProductResponse =
            await HttpClient.PostAsync(BASE_URL, productAsJson);
        var createResponseContent = await createProductResponse.Content.ReadAsStringAsync();

        var getProductsResult = await HttpClient.GetAsync(BASE_URL);

        // * Assert
        createProductResponse.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        createResponseContent.Should().Be(ErrorMessages.INVALID_PRICE);

        getProductsResult.EnsureSuccessStatusCode();
        getProductsResult.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);

        var productsString = await getProductsResult.Content.ReadAsStringAsync();
        var products = JsonSerializer.Deserialize<List<ProductDto>>(productsString);
        products.Should().BeEmpty();
    }

    [Fact]
    public async void When_CreatedProductWithExistentName_Then_ShouldReturnFailure()
    {
        // * Arrange
        CleanDatabase();
        CreateProductDto createProductDto = CreateSUT();
        StringContent productAsJson = new StringContent(
                            JsonSerializer.Serialize(createProductDto),
                            Encoding.UTF8,
                            Application.Json
                            );
        await HttpClient.PostAsync(BASE_URL, productAsJson);

        // * Act
        var createProductResponse =
            await HttpClient.PostAsync(BASE_URL, productAsJson);
        var createResponseContent = await createProductResponse.Content.ReadAsStringAsync();

        // * Assert
        createProductResponse.StatusCode.Should().Be(System.Net.HttpStatusCode.Conflict);
        createResponseContent.Should().Be(ErrorMessages.NAME_ALREADY_EXISTS);
    }

    [Fact]
    public async void When_CreatedProductWithEmptyName_Then_ShouldReturnFailureAndNotBeReturnedInGet()
    {
        // * Arrange
        CleanDatabase();
        CreateProductDto createProductDto = CreateSUT();
        createProductDto.Name = EMPTY_STRING;

        // * Act
        StringContent productAsJson = new StringContent(
                            JsonSerializer.Serialize(createProductDto),
                            Encoding.UTF8,
                            Application.Json
                            );
        var createProductResponse =
            await HttpClient.PostAsync(BASE_URL, productAsJson);
        var createResponseContent = await createProductResponse.Content.ReadAsStringAsync();

        var getProductsResult = await HttpClient.GetAsync(BASE_URL);

        // * Assert
        createProductResponse.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        createResponseContent.Should().Be(ErrorMessages.EMPTY_NAME);

        getProductsResult.EnsureSuccessStatusCode();
        getProductsResult.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);

        var productsString = await getProductsResult.Content.ReadAsStringAsync();
        var products = JsonSerializer.Deserialize<List<ProductDto>>(productsString);
        products.Should().BeEmpty();
    }

    [Fact]
    public async void When_GetAllProducts_Then_ShouldReturnProducts()
    {
        // * Arrange
        CleanDatabase();
        CreateProductDto createProductDto = CreateSUT();
        StringContent productAsJson = new StringContent(
                            JsonSerializer.Serialize(createProductDto),
                            Encoding.UTF8,
                            Application.Json
                            );
        await HttpClient.PostAsync(BASE_URL, productAsJson);

        // * Act
        var getProductsResult = await HttpClient.GetAsync(BASE_URL);
        var productsString = await getProductsResult.Content.ReadAsStringAsync();
        var products = JsonSerializer.Deserialize<List<ProductDto>>(productsString);

        // * Assert
        getProductsResult.EnsureSuccessStatusCode();
        getProductsResult.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
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
