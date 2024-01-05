using System.Collections.ObjectModel;
using ProductsAPI.Data;
using ProductsAPI.Entities;
using ProductsAPI.Helpers;

namespace ProductsAPI.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly DatabaseContext context;

    public ProductRepository(DatabaseContext context)
    {
        this.context = context;
    }

    public Result Add(Product product)
    {
        if (IsNamePresent(product.Name))
        {
            return Result.Failure("Product name already exists.");
        }
        context.Set<Product>().Add(product);
        context.SaveChanges();
        return Result.Success();
    }

    public Result DeleteById(int id)
    {
        ResultOfEntity<Product> productWrapper = GetById(id);
        if (!productWrapper.IsSuccess)
        {
            return Result.Failure(productWrapper.Error);
        }
        context.Set<Product>().Remove(productWrapper.Entity);
        context.SaveChanges();
        return Result.Success();
    }

    public IEnumerable<Product> GetAll()
    {
        IList<Product> products = context.Set<Product>().ToList();
        return new ReadOnlyCollection<Product>(products);
    }

    public ResultOfEntity<Product> GetById(int id)
    {
        Product? product = context.Set<Product>().SingleOrDefault(p => p.Id == id);
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
        context.SaveChanges();
        return Result.Success();
    }

    public bool IsNamePresent(string name)
    {
        return context.Set<Product>().Any(p => p.Name == name);
    }
}
