using System.Text;
using System.Text.Json;
using static System.Net.Mime.MediaTypeNames;
using Microsoft.AspNetCore.Mvc.Testing;

using ProductsAPI.Entities;
using ProductsAPI.Data;
using ProductsAPI.Controllers;
using ProductsAPI.Helpers;

namespace ProductsAPI.tests.Controllers;

// TODO: test GET by name method
// TODO: test PUT method
// TODO: test DELETE method

public class ProductControllerTests
{
    private static readonly string DEFAULT_PRODUCT_NAME = "Apple";
    private static readonly float DEFAULT_PRODUCT_PRICE = 0.1f;
    private static readonly float INVALID_PRODUCT_PRICE = 0f;
    private static readonly Guid INEXISTENT_ID = Guid.NewGuid();
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
        HttpResponseMessage createProductResponse = await GetPostResponse(createProductDto);

        var getProductResponse = await HttpClient.GetAsync(BASE_URL);
        List<ProductDto>? products = await GetProductsFromHttpResponse(getProductResponse);

        // * Assert
        createProductResponse.EnsureSuccessStatusCode();
        createProductResponse.StatusCode.Should().Be(System.Net.HttpStatusCode.Created);

        getProductResponse.EnsureSuccessStatusCode();
        getProductResponse.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
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
        HttpResponseMessage createProductResponse = await GetPostResponse(createProductDto);
        var createResponseContent = await createProductResponse.Content.ReadAsStringAsync();

        var getProductsResponse = await HttpClient.GetAsync(BASE_URL);
        List<ProductDto>? products = await GetProductsFromHttpResponse(getProductsResponse);

        // * Assert
        createProductResponse.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        createResponseContent.Should().Be(ErrorMessages.INVALID_PRICE);

        getProductsResponse.EnsureSuccessStatusCode();
        getProductsResponse.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        products.Should().BeEmpty();
    }

    [Fact]
    public async void When_CreatedProductWithExistentName_Then_ShouldReturnFailure()
    {
        // * Arrange
        CleanDatabase();
        CreateProductDto createProductDto = CreateSUT();
        await GetPostResponse(createProductDto);
        StringContent productAsJson = GetSerializedProduct(createProductDto);

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
        HttpResponseMessage createProductResponse = await GetPostResponse(createProductDto);
        var createResponseContent = await createProductResponse.Content.ReadAsStringAsync();

        var getProductResponse = await HttpClient.GetAsync(BASE_URL);
        List<ProductDto>? products = await GetProductsFromHttpResponse(getProductResponse);

        // * Assert
        createProductResponse.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        createResponseContent.Should().Be(ErrorMessages.EMPTY_NAME);

        getProductResponse.EnsureSuccessStatusCode();
        getProductResponse.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        products.Should().BeEmpty();
    }


    [Fact]
    public async void When_CreatedProductWithNoName_Then_ShouldReturnFailureAndNotBeReturnedInGet()
    {
        // * Arrange
        CleanDatabase();
        CreateProductDto createProductDto = new CreateProductDto
        {
            Price = DEFAULT_PRODUCT_PRICE
        };

        // * Act
        HttpResponseMessage createProductResponse = await GetPostResponse(createProductDto);

        var getProductResponse = await HttpClient.GetAsync(BASE_URL);
        List<ProductDto>? products = await GetProductsFromHttpResponse(getProductResponse);

        // * Assert
        createProductResponse.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);

        getProductResponse.EnsureSuccessStatusCode();
        getProductResponse.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        products.Should().BeEmpty();
    }

    [Fact]
    public async void When_CreatedProductWithNoPrice_Then_ShouldReturnFailureAndNotBeReturnedInGet()
    {
        // * Arrange
        CleanDatabase();
        CreateProductDto createProductDto = new CreateProductDto
        {
            Name = DEFAULT_PRODUCT_NAME
        };

        // * Act
        HttpResponseMessage createProductResponse = await GetPostResponse(createProductDto);
        var createResponseContent = await createProductResponse.Content.ReadAsStringAsync();

        var getProductResponse = await HttpClient.GetAsync(BASE_URL);
        List<ProductDto>? products = await GetProductsFromHttpResponse(getProductResponse);

        // * Assert
        createProductResponse.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        createResponseContent.Should().Be(ErrorMessages.INVALID_PRICE);

        getProductResponse.EnsureSuccessStatusCode();
        getProductResponse.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        products.Should().BeEmpty();
    }

    [Fact]
    public async void When_CreatedProductWithNoNameAndPrice_Then_ShouldReturnFailureAndNotBeReturnedInGet()
    {
        // * Arrange
        CleanDatabase();
        CreateProductDto createProductDto = new CreateProductDto();

        // * Act
        HttpResponseMessage createProductResponse = await GetPostResponse(createProductDto);

        var getProductResponse = await HttpClient.GetAsync(BASE_URL);
        List<ProductDto>? products = await GetProductsFromHttpResponse(getProductResponse);

        // * Assert
        createProductResponse.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);

        getProductResponse.EnsureSuccessStatusCode();
        getProductResponse.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        products.Should().BeEmpty();
    }

    [Fact]
    public async void When_GetAllProducts_Then_ShouldReturnProducts()
    {
        // * Arrange
        CleanDatabase();
        CreateProductDto createProductDto = CreateSUT();
        await GetPostResponse(createProductDto);

        // * Act
        var getProductResponse = await HttpClient.GetAsync(BASE_URL);
        List<ProductDto>? products = await GetProductsFromHttpResponse(getProductResponse);

        // * Assert
        getProductResponse.EnsureSuccessStatusCode();
        getProductResponse.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        products.Should().NotBeNull();
        products.Should().HaveCount(1);
    }

    [Fact]
    public async void When_GetProductById_Then_ShouldReturnProduct()
    {
        // * Arrange
        CleanDatabase();
        CreateProductDto createProductDto = CreateSUT();

        HttpResponseMessage createProductResponse = await GetPostResponse(createProductDto);
        var createdProductString = await createProductResponse.Content.ReadAsStringAsync();

        ProductDto? product = JsonSerializer.Deserialize<ProductDto>(createdProductString);
        Guid productId = product.Id;
        string getByIdUrl = $"{BASE_URL}/{productId}";

        // * Act
        var getProductResponse =
            await HttpClient.GetAsync(getByIdUrl);
        var getResponseContent = await getProductResponse.Content.ReadAsStringAsync();
        var retrievedProduct = JsonSerializer.Deserialize<ProductDto>(getResponseContent);

        // * Assert
        getProductResponse.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        retrievedProduct.Should().NotBeNull();
        retrievedProduct.Id.Should().Be(productId);
        retrievedProduct.Name.Should().Be(DEFAULT_PRODUCT_NAME);
        retrievedProduct.Price.Should().Be(DEFAULT_PRODUCT_PRICE);
    }

    [Fact]
    public async void When_GetProductByInexistentId_Then_ShouldReturnProduct()
    {
        // * Arrange
        CleanDatabase();
        string getByIdUrl = $"{BASE_URL}/{INEXISTENT_ID}";

        // * Act
        var getProductResponse =
            await HttpClient.GetAsync(getByIdUrl);
        var getResponseContent = await getProductResponse.Content.ReadAsStringAsync();

        // * Assert
        getProductResponse.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
        getResponseContent.Should().Be(ErrorMessages.PRODUCT_ID_NOT_FOUND(INEXISTENT_ID));
    }

    private CreateProductDto CreateSUT()
    {
        return new CreateProductDto
        {
            Name = DEFAULT_PRODUCT_NAME,
            Price = DEFAULT_PRODUCT_PRICE
        };
    }

    private StringContent GetSerializedProduct(CreateProductDto createProductDto)
    {
        StringContent productAsJson = new StringContent(
                            JsonSerializer.Serialize(createProductDto),
                            Encoding.UTF8,
                            Application.Json
                            );
        return productAsJson;
    }

    private async Task<HttpResponseMessage> GetPostResponse(CreateProductDto createProductDto)
    {
        StringContent productAsJson = GetSerializedProduct(createProductDto);
        var createProductResponse =
            await HttpClient.PostAsync(BASE_URL, productAsJson);
        return createProductResponse;
    }

    private async Task<List<ProductDto>?> GetProductsFromHttpResponse(HttpResponseMessage getProductResponse)
    {
        var productsString = await getProductResponse.Content.ReadAsStringAsync();
        var products = JsonSerializer.Deserialize<List<ProductDto>>(productsString);
        return products;
    }
}
