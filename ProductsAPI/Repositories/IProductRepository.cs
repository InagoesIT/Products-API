using ProductsAPI.Entities;
using ProductsAPI.Helpers;

namespace ProductsAPI.Repositories;

public interface IProductRepository
{
    IEnumerable<Product> GetAll();
    ResultOfEntity<Product> GetById(int id);
    Result Add(Product product);
    Result DeleteById(int id);
    Result Update(int id, string? name, float? price);
}
