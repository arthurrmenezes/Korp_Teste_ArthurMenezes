using Microsoft.EntityFrameworkCore.Storage;
using StockService.Domain.Entities;

namespace StockService.Infrastructure.Data.Repositories.Interfaces;

public interface IProductRepository
{
    public Task RegisterProductAsync(Product product, CancellationToken cancellationToken);

    public Task<Product?> GetProductByCodeAsync(string code, CancellationToken cancellationToken);

    public Task<Product?> GetProductByIdAsync(Guid productId, CancellationToken cancellationToken);

    public Task UpdateProductAsync(Product product, CancellationToken cancellationToken);

    public Task<int> DeductProductBalanceAsync(string productCode, int quantity, CancellationToken cancellationToken);

    public Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken);
}
