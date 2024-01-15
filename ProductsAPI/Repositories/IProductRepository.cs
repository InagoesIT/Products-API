using ProductsAPI.Entities;
using ProductsAPI.Helpers;

namespace ProductsAPI.Repositories;

public interface IProductRepository
{
    IEnumerable<Product> GetAll();
    ResultOfEntity<Product> GetById(int id);
    ResultOfEntity<Product> GetByName(String name);
    Result Add(Product product);
    Result DeleteById(int id);
    Result Update(int id, string? name, float? price);
}
