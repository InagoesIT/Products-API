namespace ProductsAPI.tests.Helpers;

public static class TestingConstants
{
    public static readonly string DEFAULT_PRODUCT_NAME = "Apple";
    public static readonly float DEFAULT_PRODUCT_PRICE = 0.1f;
    public static readonly string INEXISTENT_NAME = "NO_NAME";
    public static readonly float ZERO_PRICE = 0.0f;
    public static readonly int NEGATIVE_PRICE = -1;
    public static readonly float INVALID_PRICE = ZERO_PRICE;
    public static readonly string NEW_PRODUCT_NAME = "Peach";
    public static readonly float NEW_PRODUCT_PRICE = 9.5f;
    public static readonly Guid INEXISTENT_ID = Guid.NewGuid();
}