using System.Text.Json.Serialization;

namespace ProductsAPI.Entities;

public class CreateProductDto
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("price")]
    public float Price { get; set; }
}
