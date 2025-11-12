using StockService.Application.Services.Inputs;
using StockService.Application.Services.Interfaces;
using StockService.Application.Services.Outputs;
using StockService.Infrastructure.Data.Repositories.Interfaces;

namespace StockService.Application.Services;

public sealed class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;

    public ProductService(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<RegisterProductServiceOutput> RegisterProductServiceAsync(RegisterProductServiceInput input, CancellationToken cancellationToken)
    {
        var productCodeExists = await _productRepository.GetProductByCodeAsync(input.Code, cancellationToken);
        if (productCodeExists != null)
            throw new InvalidOperationException($"Product with code {input.Code} already exists.");

        var product = new Domain.Entities.Product(
            code: input.Code,
            description: input.Description,
            balance: input.Balance);

        await _productRepository.RegisterProductAsync(product, cancellationToken);

        var output = RegisterProductServiceOutput.Factory(
            id: product.Id.ToString(),
            code: product.Code,
            description: product.Description,
            balance: product.Balance,
            createdAt: product.CreatedAt);

        return output;
    }

    public async Task<GetProductByIdServiceOutput> GetProductByIdServiceAsync(Guid id, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetProductByIdAsync(id, cancellationToken);
        if (product is null)
            throw new KeyNotFoundException($"Product with ID {id} was not found.");

        var output = GetProductByIdServiceOutput.Factory(
            id: product.Id.ToString(),
            code: product.Code,
            description: product.Description,
            balance: product.Balance,
            createdAt: product.CreatedAt);

        return output;
    }

    public async Task<GetProductByCodeServiceOutput> GetProductByCodeServiceAsync(string code, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetProductByCodeAsync(code, cancellationToken);
        if (product is null)
            throw new KeyNotFoundException($"Product with code {code} was not found.");

        var output = GetProductByCodeServiceOutput.Factory(
            id: product.Id.ToString(),
            code: product.Code,
            description: product.Description,
            balance: product.Balance,
            createdAt: product.CreatedAt);

        return output;
    }

    public async Task IncrementBalanceByProductListServiceAsync(IncrementBalanceByProductListServiceInput input, CancellationToken cancellationToken)
    {
        await using var transaction = await _productRepository.BeginTransactionAsync(cancellationToken);

        try
        {
            foreach (var item in input.ProductList)
            {
                var product = await _productRepository.GetProductForUpdateAsync(item.Code, cancellationToken);
                if (product is null)
                    throw new KeyNotFoundException($"Product with code {item.Code} was not found.");

                product.IncrementBalance(item.Quantity);

                await _productRepository.UpdateProductAsync(product, cancellationToken);
            }
            await transaction.CommitAsync(cancellationToken);
        }
        catch (Exception)
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    public async Task DeductBalanceByProductListServiceAsync(DeductBalanceByProductListServiceInput input, CancellationToken cancellationToken)
    {
        await using var transaction = await _productRepository.BeginTransactionAsync(cancellationToken);

        try
        {
            foreach (var item in input.ProductList)
            {
                var product = await _productRepository.GetProductForUpdateAsync(item.Code, cancellationToken);
                if (product is null)
                    throw new KeyNotFoundException($"Product with code {item.Code} was not found.");

                product.DecrementBalance(item.Quantity);

                await _productRepository.UpdateProductAsync(product, cancellationToken);
            }
            await transaction.CommitAsync(cancellationToken);
        }
        catch (Exception)
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }
}
