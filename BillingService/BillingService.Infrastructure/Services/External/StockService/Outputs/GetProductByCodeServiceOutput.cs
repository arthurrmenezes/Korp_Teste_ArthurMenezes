using System.Text.Json.Serialization;

namespace BillingService.Infrastructure.Services.External.StockService.Outputs;

public sealed class GetProductByCodeServiceOutput
{
    [JsonPropertyName("id")]
    public string ProductId { get; }
    [JsonPropertyName("code")]
    public string ProductCode { get; }
    [JsonPropertyName("description")]
    public string ProductDescription { get; }
    [JsonPropertyName("balance")]
    public int ProductBalance { get; }
    [JsonPropertyName("createdAt")]
    public DateTime ProductCreatedAt { get; }

    public GetProductByCodeServiceOutput(string productId, string productCode, string productDescription, int productBalance, DateTime productCreatedAt)
    {
        ProductId = productId;
        ProductCode = productCode;
        ProductDescription = productDescription;
        ProductBalance = productBalance;
        ProductCreatedAt = productCreatedAt;
    }
}
