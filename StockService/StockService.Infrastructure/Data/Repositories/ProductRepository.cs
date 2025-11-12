using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using StockService.Domain.Entities;
using StockService.Infrastructure.Data.Repositories.Interfaces;
using Npgsql;

namespace StockService.Infrastructure.Data.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly DataContext _dataContext;

    public ProductRepository(DataContext dataContext)
    {
        _dataContext = dataContext;
    }

    public async Task RegisterProductAsync(Product product, CancellationToken cancellationToken)
    {
        await _dataContext.Products.AddAsync(product, cancellationToken);
        await _dataContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<Product?> GetProductByCodeAsync(string code, CancellationToken cancellationToken)
    {
        var product = await _dataContext.Products
            .FirstOrDefaultAsync(p => p.Code == code, cancellationToken);
        return product;
    }

    public async Task<Product?> GetProductByIdAsync(Guid productId, CancellationToken cancellationToken)
    {
        var product = await _dataContext.Products
            .FirstOrDefaultAsync(p => p.Id == productId, cancellationToken);
        return product;
    }

    public async Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken)
    {
        return await _dataContext.Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task<Product?> GetProductForUpdateAsync(string code, CancellationToken cancellationToken)
    {
        var sql = @"SELECT * FROM ""Products"" WHERE ""Code"" = @code FOR UPDATE";
        return await _dataContext.Products
            .FromSqlRaw(sql, new NpgsqlParameter("@code", code))
            .AsTracking()
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task UpdateProductAsync(Product product, CancellationToken cancellationToken)
    {
        _dataContext.Products.Update(product);
        await _dataContext.SaveChangesAsync();
    }
}
