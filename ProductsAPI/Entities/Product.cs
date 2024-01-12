using ProductsAPI.Helpers;

namespace ProductsAPI.Entities;

public class Product
{
    private static int NextId = 0;
    public int Id { get; private set; }
    public string Name { get; set; }
    public float Price { get; private set; }

    private Product(int id, string name, float price)
    {
        Id = id;
        Name = name;
        Price = price;
    }

    public static ResultOfEntity<Product> Create(string name, float price)
    {
        if (!IsPriceValid(price))
        {
            return ResultOfEntity<Product>.Failure(ErrorMessages.INVALID_PRICE);
        }
        Product product = new Product(NextId, name, price);
        NextId++;
        return ResultOfEntity<Product>.Success(product);
    }

    public static bool IsPriceValid(float price)
    {
        return price > 0;
    }

    public Result UpdatePrice(float price)
    {
        if (!IsPriceValid(price))
        {
            return Result.Failure(ErrorMessages.INVALID_PRICE);
        }
        Price = price;

        return Result.Success();
    }
}