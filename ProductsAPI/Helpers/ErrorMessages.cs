namespace ProductsAPI.Helpers;

public class ErrorMessages
{
    public static readonly string INVALID_PRICE = "The price must be greater than zero.";
    public static readonly string NAME_ALREADY_EXISTS = "Product name already exists.";
    public static string PRODUCT_ID_NOT_FOUND(int id) => $"No product found with the id = {id}";
    public static string PRODUCT_NAME_NOT_FOUND(String name) => $"No product found with the name = {name}";

    private ErrorMessages() { }
}
