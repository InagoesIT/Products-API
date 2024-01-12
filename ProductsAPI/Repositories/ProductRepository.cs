using System.Collections.ObjectModel;
using ProductsAPI.Data;
using ProductsAPI.Entities;
using ProductsAPI.Helpers;

namespace ProductsAPI.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly DatabaseContext DatabaseContext;

    public ProductRepository(DatabaseContext context)
    {
        this.DatabaseContext = context;
    }

    public Result Add(Product product)
    {
        if (IsNamePresent(product.Name))
        {
            return Result.Failure(ErrorMessages.NAME_ALREADY_EXISTS);
        }
        DatabaseContext.Set<Product>().Add(product);
        DatabaseContext.SaveChanges();
        return Result.Success();
    }

    public Result DeleteById(int id)
    {
        ResultOfEntity<Product> productWrapper = GetById(id);
        if (!productWrapper.IsSuccess)
        {
            return Result.Failure(productWrapper.Error);
        }
        DatabaseContext.Set<Product>().Remove(productWrapper.Entity);
        DatabaseContext.SaveChanges();
        return Result.Success();
    }

    public IEnumerable<Product> GetAll()
    {
        IList<Product> products = DatabaseContext.Set<Product>().ToList();
        return new ReadOnlyCollection<Product>(products);
    }

    public ResultOfEntity<Product> GetById(int id)
    {
        Product? product = DatabaseContext.Set<Product>().SingleOrDefault(p => p.Id == id);
        if (product is null)
        {
            string errorMessage = ErrorMessages.PRODUCT_ID_NOT_FOUND(id);
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
        DatabaseContext.SaveChanges();
        return Result.Success();
    }

    public bool IsNamePresent(string name)
    {
        return DatabaseContext.Set<Product>().Any(p => p.Name == name);
    }
}
