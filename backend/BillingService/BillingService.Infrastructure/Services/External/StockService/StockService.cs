using BillingService.Infrastructure.Services.External.StockService.Inputs;
using BillingService.Infrastructure.Services.External.StockService.Interfaces;
using BillingService.Infrastructure.Services.External.StockService.Outputs;
using System.Net;
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
        if (response.StatusCode == HttpStatusCode.NotFound)
            return null;

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
            throw new HttpRequestException($"Error fetching product '{code}': {error}");
        }

        var output = await response.Content.ReadFromJsonAsync<GetProductByCodeServiceOutput>(cancellationToken: cancellationToken);
        if (output is null)
            throw new InvalidOperationException($"Invalid response from StockService for product with code '{code}'.");

        return output;
    }

    public async Task DeductBalanceByProductListServiceAsync(DeductBalanceByProductListServiceInput input, CancellationToken cancellationToken)
    {
        var endpoint = $"api/v1/products/deduct-balance";

        var response = await _httpClient.PostAsJsonAsync(endpoint, input, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new HttpRequestException($"Failed to deduct product balance: {error}");
        }
    }
}
