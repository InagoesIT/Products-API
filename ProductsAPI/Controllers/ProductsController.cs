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
    public IActionResult Create([FromBody] CreateProductDto createProduct)
    {
        string errorMessage = getErrorMessageIfHasNullFields(createProduct);
        if (!string.IsNullOrEmpty(errorMessage))
        {
            return BadRequest(errorMessage);
        }
        ResultOfEntity<Product> productWrapper = Product.Create(
            name: createProduct.Name, price: createProduct.Price.Value);
        if (!productWrapper.IsSuccess)
        {
            return BadRequest(productWrapper.Error);
        }
        Product product = productWrapper.Entity;
        Result addResult = Repository.Add(product);
        if (!addResult.IsSuccess)
        {
            if (addResult.Error == ErrorMessages.NAME_ALREADY_EXISTS)
            {
                return Conflict(addResult.Error);
            }
            return BadRequest(addResult.Error);
        }

        ProductDto productDto = new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Price = product.Price
        };
        return Created(nameof(GetAll), productDto);
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        var products = Repository.GetAll().Select(p => new ProductDto()
        {
            Id = p.Id,
            Name = p.Name,
            Price = p.Price
        });
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
        ProductDto productDto = new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Price = product.Price
        };
        return Ok(productDto);
    }

    // TODO: Maybe interdict updating with the same name/price??
    [HttpPut("{id:guid}")]
    public IActionResult UpdateById(Guid id, [FromBody] CreateProductDto updateProductDto)
    {
        if (string.Equals(updateProductDto.Name, string.Empty))
        {
            return BadRequest(ErrorMessages.EMPTY_NAME);
        }
        Result result = Repository.Update(id, updateProductDto.Name, updateProductDto.Price);
        if (!result.IsSuccess)
        {
            return GetActionResultForUpdateResult(result);
        }
        ResultOfEntity<Product> getByIdResult = Repository.GetById(id);
        Product product = getByIdResult.Entity;
        ProductDto productDto = new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Price = product.Price
        };
        return Ok(productDto);
    }

    private string getErrorMessageIfHasNullFields(CreateProductDto createProductDto)
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

    private IActionResult GetActionResultForUpdateResult(Result result)
    {
        if (result.Error.Contains(ErrorMessages.INVALID_PRICE))
        {
            return BadRequest(result.Error);
        }
        return NotFound(result.Error);
    }

}
