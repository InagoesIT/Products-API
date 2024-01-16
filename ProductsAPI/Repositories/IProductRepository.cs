using ProductsAPI.Entities;
using ProductsAPI.Helpers;

namespace ProductsAPI.Repositories;

public interface IProductRepository
{
    IEnumerable<Product> GetAll();
    ResultOfEntity<Product> GetById(Guid id);
    ResultOfEntity<Product> GetByName(String name);
    Result Add(Product product);
    Result DeleteById(Guid id);
    Result Update(Guid id, string? name, float? price);
}
