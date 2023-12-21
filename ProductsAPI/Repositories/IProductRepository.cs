using Products_API.Entities;
using Products_API.Helpers;

namespace Products_API.src.Repositories;

public interface IProductRepository
{
    IEnumerable<Product> GetAll();
    ResultOfEntity<Product> GetById(int id);
    Result Add(Product product);
    Result DeleteById(int id);
    Result Update(int id, string? name, float? price);
}
