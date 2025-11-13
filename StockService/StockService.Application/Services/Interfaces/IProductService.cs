using StockService.Application.Services.Inputs;
using StockService.Application.Services.Outputs;

namespace StockService.Application.Services.Interfaces;

public interface IProductService
{
    public Task<RegisterProductServiceOutput> RegisterProductServiceAsync(
        RegisterProductServiceInput input,
        CancellationToken cancellationToken);

    public Task<GetProductByIdServiceOutput> GetProductByIdServiceAsync(
        Guid id,
        CancellationToken cancellationToken);

    public Task<GetProductByCodeServiceOutput> GetProductByCodeServiceAsync(
        string code,
        CancellationToken cancellationToken);

    public Task IncrementBalanceByProductListServiceAsync(
        IncrementBalanceByProductListServiceInput input,
        CancellationToken cancellationToken);

    public Task DeductBalanceByProductListServiceAsync(
        DeductBalanceByProductListServiceInput input,
        CancellationToken cancellationToken);

    public Task<GetAllProductsServiceOutput> GetAllProductsServiceAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken);
}
