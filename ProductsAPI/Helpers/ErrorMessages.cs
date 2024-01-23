namespace ProductsAPI.Helpers;

public static class ErrorMessages
{
    public static readonly string INVALID_PRICE = "The price must be greater than zero.";
    public static readonly string NAME_ALREADY_EXISTS = "Product name already exists.";
    public static readonly string EMPTY_NAME = "Product name can't be empty.";
    public static readonly string NAME_NOT_GIVEN = "Product name must be given.";
    public static readonly string PRICE_NOT_GIVEN = "Product price must be given.";
    public static readonly string NAME_AND_PRICE_NOT_GIVEN = "Product name and price must be given.";
    public static readonly string PRODUCT_ID_NOT_FOUND_WITHOUT_ID = "No product found with the id";
    public static string PRODUCT_ID_NOT_FOUND(Guid id) => $"{PRODUCT_ID_NOT_FOUND_WITHOUT_ID} = {id}";
    public static string PRODUCT_NAME_NOT_FOUND(string name) => $"No product found with the name = {name}";
}
