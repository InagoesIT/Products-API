using Microsoft.AspNetCore.Mvc;

using ProductsAPI.Repositories;
using ProductsAPI.Entities;
using ProductsAPI.Helpers;

namespace ProductsAPI.Controllers;

[ApiController]
[Route("/api/v1/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductRepository Repository;

    public ProductsController(IProductRepository repository)
    {
        this.Repository = repository;
    }

    [HttpPost]
    public IActionResult Create([FromBody] CreateProductDto createProduct)
    {
        ResultOfEntity<Product> productWrapper = Product.Create(
            name: createProduct.Name, price: createProduct.Price);
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
        ProductDto productDto = new ProductDto {
            Id = product.Id,
            Name = product.Name,
            Price = product.Price
        };
        return Ok(productDto);
    }

}
