using System.Text.Json.Serialization;

namespace ProductsAPI.Entities;

public class ProductDto : CreateProductDto
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
}
