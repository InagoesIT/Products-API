using Microsoft.AspNetCore.Mvc;

using ProductsAPI.Repositories;
using ProductsAPI.Entities;
using ProductsAPI.Helpers;

namespace ProductsAPI.Controllers;

[ApiController]
[Route("/api/v1/[controller]")]
public class ProductsController(IProductRepository repository) : ControllerBase
{
    private readonly IProductRepository Repository = repository;

    [HttpPost]
    public IActionResult Create([FromBody] CreateProductDto createProductDto)
    {
        ResultOfEntity<Product> productWrapper = GetResultForCreateProduct(createProductDto);
        if (!productWrapper.IsSuccess)
        {
            return BadRequest(productWrapper.Error);
        }

        Product product = productWrapper.Entity;
        Result addResult = Repository.Add(product);
        if (!addResult.IsSuccess)
        {
            return GetActionResultForErrorInAdd(addResult.Error);
        }

        ProductDto productDto = MapToProductDto(product);
        return Created(nameof(GetAll), productDto);
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        var products = Repository.GetAll().Select(MapToProductDto);
        return Ok(products);
    }

    [HttpGet("{id:guid}")]
    public IActionResult GetById(Guid id)
    {
        ResultOfEntity<Product> result = Repository.GetById(id);
        if (!result.IsSuccess)
        {
            return NotFound(result.Error);
        }
        Product product = result.Entity;
        ProductDto productDto = MapToProductDto(product);
        return Ok(productDto);
    }

    // TODO: Maybe interdict updating with the same name/price??
    [HttpPut("{id:guid}")]
    public IActionResult UpdateById(Guid id, [FromBody] CreateProductDto updateProductDto)
    {
        Result result = GetResultForUpdateProduct(id, updateProductDto);
        if (!result.IsSuccess)
        {
            return GetActionResultForUpdateResult(result);
        }
        ResultOfEntity<Product> getByIdResult = Repository.GetById(id);
        Product updatedProduct = getByIdResult.Entity;
        ProductDto updatedProductDto = MapToProductDto(updatedProduct);
        return Ok(updatedProductDto);
    }

    private ResultOfEntity<Product> GetResultForCreateProduct(CreateProductDto createProductDto)
    {
        string errorMessage = GetErrorMessageIfHasNullFields(createProductDto);
        if (!string.IsNullOrEmpty(errorMessage))
        {
            return ResultOfEntity<Product>.Failure(errorMessage);
        }
        ResultOfEntity<Product> productWrapper = Product.Create(
            name: createProductDto.Name, price: createProductDto.Price.Value);
        return productWrapper;
    }

    private string GetErrorMessageIfHasNullFields(CreateProductDto createProductDto)
    {
        bool nameIsNull = createProductDto.Name is null;
        bool priceIsNull = !createProductDto.Price.HasValue;
        if (nameIsNull && priceIsNull)
        {
            return ErrorMessages.NAME_AND_PRICE_NOT_GIVEN;
        }
        else if (nameIsNull)
        {
            return ErrorMessages.NAME_NOT_GIVEN;
        }
        else if (priceIsNull)
        {
            return ErrorMessages.PRICE_NOT_GIVEN;
        }
        return string.Empty;
    }

    private IActionResult GetActionResultForErrorInAdd(string error)
    {
        if (error == ErrorMessages.NAME_ALREADY_EXISTS)
        {
            return Conflict(error);
        }
        return BadRequest(error);
    }

    private ProductDto MapToProductDto(Product product)
    {
        return new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Price = product.Price
        };
    }

    private Result GetResultForUpdateProduct(Guid id, CreateProductDto updateProductDto)
    {
        if (string.Equals(updateProductDto.Name, string.Empty))
        {
            return Result.Failure(ErrorMessages.EMPTY_NAME);
        }
        Result result = Repository.Update(id, updateProductDto.Name, updateProductDto.Price);
        return result;
    }

    private IActionResult GetActionResultForUpdateResult(Result result)
    {
        if (result.Error.Contains(ErrorMessages.PRODUCT_ID_NOT_FOUND_WITHOUT_ID))
        {
            return NotFound(result.Error);
        }
        return BadRequest(result.Error);
    }
}
