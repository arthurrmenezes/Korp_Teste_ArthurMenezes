using BillingService.Infrastructure.Services.External.StockService.Inputs;
using BillingService.Infrastructure.Services.External.StockService.Outputs;

namespace BillingService.Infrastructure.Services.External.StockService.Interfaces;

public interface IStockService
{
    public Task<GetProductByCodeServiceOutput?> GetProductByCodeServiceAsync(string code, CancellationToken cancellationToken);
    public Task DeductBalanceByProductListServiceAsync(DeductBalanceByProductListServiceInput input, CancellationToken cancellationToken);
}
