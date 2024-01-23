using System.Text;
using System.Text.Json;
using static System.Net.Mime.MediaTypeNames;
using Microsoft.AspNetCore.Mvc.Testing;

using ProductsAPI.Entities;
using ProductsAPI.Data;
using ProductsAPI.Controllers;
using ProductsAPI.Helpers;
using ProductsAPI.tests.Helpers;

namespace ProductsAPI.tests.Controllers;


// TODO: test DELETE method
public class ProductControllerTests
{
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
        DatabaseContext databaseContext = new();
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
        createProductDto.Price = TestingConstants.INVALID_PRICE;

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
        createProductDto.Name = string.Empty;

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
            Price = TestingConstants.DEFAULT_PRODUCT_PRICE
        };

        // * Act
        HttpResponseMessage createProductResponse = await GetPostResponse(createProductDto);
        var createResponseContent = await createProductResponse.Content.ReadAsStringAsync();

        var getProductResponse = await HttpClient.GetAsync(BASE_URL);
        List<ProductDto>? products = await GetProductsFromHttpResponse(getProductResponse);

        // * Assert
        createProductResponse.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        createResponseContent.Should().Be(ErrorMessages.NAME_NOT_GIVEN);

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
            Name = TestingConstants.DEFAULT_PRODUCT_NAME
        };

        // * Act
        HttpResponseMessage createProductResponse = await GetPostResponse(createProductDto);
        var createResponseContent = await createProductResponse.Content.ReadAsStringAsync();

        var getProductResponse = await HttpClient.GetAsync(BASE_URL);
        List<ProductDto>? products = await GetProductsFromHttpResponse(getProductResponse);

        // * Assert
        createProductResponse.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        createResponseContent.Should().Be(ErrorMessages.PRICE_NOT_GIVEN);

        getProductResponse.EnsureSuccessStatusCode();
        getProductResponse.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        products.Should().BeEmpty();
    }

    [Fact]
    public async void When_CreatedProductWithNoNameAndNoPrice_Then_ShouldReturnFailureAndNotBeReturnedInGet()
    {
        // * Arrange
        CleanDatabase();
        CreateProductDto createProductDto = new();

        // * Act
        HttpResponseMessage createProductResponse = await GetPostResponse(createProductDto);
        var createResponseContent = await createProductResponse.Content.ReadAsStringAsync();

        var getProductResponse = await HttpClient.GetAsync(BASE_URL);
        List<ProductDto>? products = await GetProductsFromHttpResponse(getProductResponse);

        // * Assert
        createProductResponse.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        createResponseContent.Should().Be(ErrorMessages.NAME_AND_PRICE_NOT_GIVEN);

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
        Guid productId = await GetIdOfCreatedProduct();
        string productUrl = GetProductUrlFromId(productId);

        // * Act
        var getProductResponse =
            await HttpClient.GetAsync(productUrl);
        var getResponseContent = await getProductResponse.Content.ReadAsStringAsync();
        var retrievedProduct = JsonSerializer.Deserialize<ProductDto>(getResponseContent);

        // * Assert
        getProductResponse.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        retrievedProduct.Should().NotBeNull();
        retrievedProduct.Id.Should().Be(productId);
        retrievedProduct.Name.Should().Be(TestingConstants.DEFAULT_PRODUCT_NAME);
        retrievedProduct.Price.Should().Be(TestingConstants.DEFAULT_PRODUCT_PRICE);
    }

    [Fact]
    public async void When_GetProductByInexistentId_Then_ShouldReturnProduct()
    {
        // * Arrange
        CleanDatabase();
        string getByIdUrl = GetProductUrlFromId(TestingConstants.INEXISTENT_ID);

        // * Act
        var getProductResponse =
            await HttpClient.GetAsync(getByIdUrl);
        var getResponseContent = await getProductResponse.Content.ReadAsStringAsync();

        // * Assert
        getProductResponse.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
        getResponseContent.Should().Be(ErrorMessages.PRODUCT_ID_NOT_FOUND(TestingConstants.INEXISTENT_ID));
    }

    [Fact]
    public async void When_UpdateProductWithName_Then_ShouldReturnProduct()
    {
        // * Arrange
        CleanDatabase();
        Guid productId = await GetIdOfCreatedProduct();
        string productUrl = GetProductUrlFromId(productId);

        CreateProductDto newProductInfo = new CreateProductDto
        {
            Name = TestingConstants.NEW_PRODUCT_NAME
        };
        StringContent newProductInfoAsContent = GetSerializedProduct(newProductInfo);

        // * Act
        var putProductResponse =
            await HttpClient.PutAsync(productUrl, newProductInfoAsContent);
        var putResponseContent = await putProductResponse.Content.ReadAsStringAsync();
        var retrievedProduct = JsonSerializer.Deserialize<ProductDto>(putResponseContent);

        // * Assert
        putProductResponse.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        retrievedProduct.Should().NotBeNull();
        retrievedProduct.Id.Should().Be(productId);
        retrievedProduct.Name.Should().Be(TestingConstants.NEW_PRODUCT_NAME);
        retrievedProduct.Price.Should().Be(TestingConstants.DEFAULT_PRODUCT_PRICE);
    }

    [Fact]
    public async void When_UpdateProductWithPrice_Then_ShouldReturnProduct()
    {
        // * Arrange
        CleanDatabase();
        Guid productId = await GetIdOfCreatedProduct();
        string productUrl = GetProductUrlFromId(productId);

        CreateProductDto newProductInfo = new CreateProductDto
        {
            Price = TestingConstants.NEW_PRODUCT_PRICE
        };
        StringContent newProductInfoAsContent = GetSerializedProduct(newProductInfo);

        // * Act
        var putProductResponse =
            await HttpClient.PutAsync(productUrl, newProductInfoAsContent);
        var putResponseContent = await putProductResponse.Content.ReadAsStringAsync();
        var retrievedProduct = JsonSerializer.Deserialize<ProductDto>(putResponseContent);

        // * Assert
        putProductResponse.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        retrievedProduct.Should().NotBeNull();
        retrievedProduct.Id.Should().Be(productId);
        retrievedProduct.Name.Should().Be(TestingConstants.DEFAULT_PRODUCT_NAME);
        retrievedProduct.Price.Should().Be(TestingConstants.NEW_PRODUCT_PRICE);
    }

    [Fact]
    public async void When_UpdateProductWithNameAndPrice_Then_ShouldReturnProduct()
    {
        // * Arrange
        CleanDatabase();
        Guid productId = await GetIdOfCreatedProduct();
        string productUrl = GetProductUrlFromId(productId);

        CreateProductDto newProductInfo = new CreateProductDto
        {
            Name = TestingConstants.NEW_PRODUCT_NAME,
            Price = TestingConstants.NEW_PRODUCT_PRICE
        };
        StringContent newProductInfoAsContent = GetSerializedProduct(newProductInfo);

        // * Act
        var putProductResponse =
            await HttpClient.PutAsync(productUrl, newProductInfoAsContent);
        var putResponseContent = await putProductResponse.Content.ReadAsStringAsync();
        var retrievedProduct = JsonSerializer.Deserialize<ProductDto>(putResponseContent);

        // * Assert
        putProductResponse.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        retrievedProduct.Should().NotBeNull();
        retrievedProduct.Id.Should().Be(productId);
        retrievedProduct.Name.Should().Be(TestingConstants.NEW_PRODUCT_NAME);
        retrievedProduct.Price.Should().Be(TestingConstants.NEW_PRODUCT_PRICE);
    }

    [Fact]
    public async void When_UpdateProductWithNameAndInvalidPrice_Then_ShouldReturnFailure()
    {
        // * Arrange
        CleanDatabase();
        Guid productId = await GetIdOfCreatedProduct();
        string productUrl = GetProductUrlFromId(productId);

        CreateProductDto newProductInfo = new CreateProductDto
        {
            Name = TestingConstants.NEW_PRODUCT_NAME,
            Price = TestingConstants.INVALID_PRICE
        };
        StringContent newProductInfoAsContent = GetSerializedProduct(newProductInfo);

        // * Act
        var putProductResponse =
            await HttpClient.PutAsync(productUrl, newProductInfoAsContent);
        var putResponseContent = await putProductResponse.Content.ReadAsStringAsync();

        // * Assert
        putProductResponse.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        putResponseContent.Should().Be(ErrorMessages.INVALID_PRICE);
    }

    [Fact]
    public async void When_UpdateProductWithEmptyNameAndPrice_Then_ShouldReturnFailure()
    {
        // * Arrange
        CleanDatabase();
        Guid productId = await GetIdOfCreatedProduct();
        string productUrl = GetProductUrlFromId(productId);

        CreateProductDto newProductInfo = new CreateProductDto
        {
            Name = string.Empty,
            Price = TestingConstants.DEFAULT_PRODUCT_PRICE
        };
        StringContent newProductInfoAsContent = GetSerializedProduct(newProductInfo);

        // * Act
        var putProductResponse =
            await HttpClient.PutAsync(productUrl, newProductInfoAsContent);
        var putResponseContent = await putProductResponse.Content.ReadAsStringAsync();

        // * Assert
        putProductResponse.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        putResponseContent.Should().Be(ErrorMessages.EMPTY_NAME);
    }

    [Fact]
    public async void When_UpdateProductWithEmptyName_Then_ShouldReturnFailure()
    {
        // * Arrange
        CleanDatabase();
        Guid productId = await GetIdOfCreatedProduct();
        string productUrl = GetProductUrlFromId(productId);

        CreateProductDto newProductInfo = new CreateProductDto
        {
            Name = string.Empty
        };
        StringContent newProductInfoAsContent = GetSerializedProduct(newProductInfo);

        // * Act
        var putProductResponse =
            await HttpClient.PutAsync(productUrl, newProductInfoAsContent);
        var putResponseContent = await putProductResponse.Content.ReadAsStringAsync();

        // * Assert
        putProductResponse.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        putResponseContent.Should().Be(ErrorMessages.EMPTY_NAME);
    }

    [Fact]
    public async void When_UpdateProductWithInvalidPrice_Then_ShouldReturnFailure()
    {
        // * Arrange
        CleanDatabase();
        Guid productId = await GetIdOfCreatedProduct();
        string productUrl = GetProductUrlFromId(productId);

        CreateProductDto newProductInfo = new CreateProductDto
        {
            Price = TestingConstants.INVALID_PRICE
        };
        StringContent newProductInfoAsContent = GetSerializedProduct(newProductInfo);

        // * Act
        var putProductResponse =
            await HttpClient.PutAsync(productUrl, newProductInfoAsContent);
        var putResponseContent = await putProductResponse.Content.ReadAsStringAsync();

        // * Assert
        putProductResponse.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        putResponseContent.Should().Be(ErrorMessages.INVALID_PRICE);
    }

    [Fact]
    public async void When_UpdateProductWithInvalidId_Then_ShouldReturnFailure()
    {
        // * Arrange
        CleanDatabase();
        Guid productId = TestingConstants.INEXISTENT_ID;
        string productUrl = GetProductUrlFromId(productId);

        CreateProductDto newProductInfo = new CreateProductDto();
        StringContent newProductInfoAsContent = GetSerializedProduct(newProductInfo);

        // * Act
        var putProductResponse =
            await HttpClient.PutAsync(productUrl, newProductInfoAsContent);
        var putResponseContent = await putProductResponse.Content.ReadAsStringAsync();

        // * Assert
        putProductResponse.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
        putResponseContent.Should().Be(ErrorMessages.PRODUCT_ID_NOT_FOUND(productId));
    }

    private CreateProductDto CreateSUT()
    {
        return new CreateProductDto
        {
            Name = TestingConstants.DEFAULT_PRODUCT_NAME,
            Price = TestingConstants.DEFAULT_PRODUCT_PRICE
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

    private async Task<Guid> GetIdOfCreatedProduct()
    {
        CreateProductDto createProductDto = CreateSUT();
        HttpResponseMessage createProductResponse = await GetPostResponse(createProductDto);
        Guid productId = await GetIdFromResponse(createProductResponse);
        return productId;
    }

    private async Task<Guid> GetIdFromResponse(HttpResponseMessage createProductResponse)
    {
        var createdProductString = await createProductResponse.Content.ReadAsStringAsync();
        ProductDto? product = JsonSerializer.Deserialize<ProductDto>(createdProductString);
        Guid productId = product.Id;
        return productId;
    }

    private string GetProductUrlFromId(Guid productId)
    {
        return $"{BASE_URL}/{productId}";
    }

    private async Task<List<ProductDto>?> GetProductsFromHttpResponse(HttpResponseMessage getProductResponse)
    {
        var productsString = await getProductResponse.Content.ReadAsStringAsync();
        var products = JsonSerializer.Deserialize<List<ProductDto>>(productsString);
        return products;
    }
}
