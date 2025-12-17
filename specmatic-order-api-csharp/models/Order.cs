using System.Text.Json.Serialization;
using System.Diagnostics.CodeAnalysis;
namespace specmatic_order_api_csharp.models;

[ExcludeFromCodeCoverage]
public class Order
{
    private static int _idCounter = 0;

    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("productid")]
    public int Productid { get; set; } 

    [JsonPropertyName("count")]
    public int Count { get; set; }

    [JsonPropertyName("status")]
    [JsonConverter(typeof(StrictStringEnumConverter<OrderStatus>))]
    public OrderStatus Status { get; set; } 

    public Order()
    {
        Id = _idCounter++;  // Simple ID generation (increment and assign)
    }
}
public enum OrderStatus
{
    pending,
    fulfilled,
    cancelled
}

