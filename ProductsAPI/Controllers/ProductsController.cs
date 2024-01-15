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
            return BadRequest(addResult.Error);
        }

        ProductDto productDto = new ProductDto
        {
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

}
