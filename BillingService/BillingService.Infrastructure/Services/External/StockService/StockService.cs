using BillingService.Infrastructure.Services.External.StockService.Inputs;
using BillingService.Infrastructure.Services.External.StockService.Interfaces;
using BillingService.Infrastructure.Services.External.StockService.Outputs;
using System.Net.Http.Json;

namespace BillingService.Infrastructure.Services.External.StockService;

public sealed class StockService : IStockService
{
    private readonly HttpClient _httpClient;

    public StockService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<GetProductByCodeServiceOutput?> GetProductByCodeServiceAsync(string code, CancellationToken cancellationToken)
    {
        var endpoint = $"api/v1/products/code/{code}";

        var response = await _httpClient.GetAsync(endpoint, cancellationToken);
        if (!response.IsSuccessStatusCode)
            return null;

        var output = await response.Content.ReadFromJsonAsync<GetProductByCodeServiceOutput>(cancellationToken: cancellationToken);
        
        return output;
    }

    public async Task DeductBalanceByProductListServiceAsync(DeductBalanceByProductListServiceInput input, CancellationToken cancellationToken)
    {
        var endpoint = $"api/v1/products/deduct-balance";

        var response = await _httpClient.PostAsJsonAsync(endpoint, input, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            var errorBody = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new HttpRequestException($"Failed to deduct product balance.");
        }
    }
}
