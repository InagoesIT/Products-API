using ProductsAPI.Entities;
using ProductsAPI.Helpers;

namespace ProductsAPI.src.Repositories;

public class ProductRepository : IProductRepository
{
    private static readonly List<Product> _products = [];

    public Result Add(Product product)
    {
        if (IsNamePresent(product.Name))
        {
            return Result.Failure("Product name already exists.");
        }
        _products.Add(product);
        return Result.Success();
    }

    public Result DeleteById(int id)
    {
        ResultOfEntity<Product> productWrapper = GetById(id);
        if (!productWrapper.IsSuccess)
        {
            return Result.Failure(productWrapper.Error);
        }
        _products.Remove(productWrapper.Entity);
        return Result.Success();
    }

    public IEnumerable<Product> GetAll()
    {
        return _products.AsReadOnly();
    }

    public ResultOfEntity<Product> GetById(int id)
    {
        Product? product = _products.SingleOrDefault(p => p.Id == id);
        if (product is null)
        {
            string errorMessage = $"No product found with the id = {id}";
            return ResultOfEntity<Product>.Failure(errorMessage);
        }
        return ResultOfEntity<Product>.Success(product);
    }

    public Result Update(int id, string? name, float? price)
    {
        ResultOfEntity<Product> productWrapper = GetById(id);
        if (!productWrapper.IsSuccess)
        {
            return Result.Failure(productWrapper.Error);
        }
        Product product = productWrapper.Entity;
        if (name is not null)
        {
            product.Name = name;
        }
        if (price.HasValue)
        {
            return product.UpdatePrice(price.Value);
        }
        return Result.Success();
    }

    public bool IsNamePresent(string name)
    {
        return _products.Exists(p => p.Name == name);
    }
}
