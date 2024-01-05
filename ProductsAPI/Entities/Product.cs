using ProductsAPI.Helpers;

namespace ProductsAPI.Entities;

public class Product
{
    public static readonly string INVALID_PRICE_MESSAGE = "The price must be greater than zero.";
    private static int nextId = 0;
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
            return ResultOfEntity<Product>.Failure(INVALID_PRICE_MESSAGE);
        }
        Product product = new Product(nextId, name, price);
        nextId++;
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
            return Result.Failure(INVALID_PRICE_MESSAGE);
        }
        Price = price;
        
        return Result.Success();
    }
}