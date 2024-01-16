namespace ProductsAPI.Helpers;

public class ErrorMessages
{
    public static readonly string INVALID_PRICE = "The price must be greater than zero.";
    public static readonly string NAME_ALREADY_EXISTS = "Product name already exists.";
    public static readonly string EMPTY_NAME = "Product name can't be empty.";
    public static string PRODUCT_ID_NOT_FOUND(Guid id) => $"No product found with the id = {id}";
    public static string PRODUCT_NAME_NOT_FOUND(string name) => $"No product found with the name = {name}";

    private ErrorMessages() { }
}
