using BillingService.Domain.Entities;
using Microsoft.EntityFrameworkCore.Storage;

namespace BillingService.Infrastructure.Data.Repositories.Interfaces;

public interface IInvoiceRepository
{
    public Task RegisterInvoiceAsync(Invoice invoice, CancellationToken cancellationToken);
    public Task<Invoice?> GetInvoiceByIdAsync(int id, CancellationToken cancellationToken);
    public Task AddProductToInvoiceAsync(Invoice invoice, CancellationToken cancellationToken);
    public Task UpdateInvoiceAsync(Invoice invoice, CancellationToken cancellationToken);
    public Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken);
    public Task<Invoice?> GetInvoiceToUpdateByIdAsync(int id, CancellationToken cancellationToken);
}
