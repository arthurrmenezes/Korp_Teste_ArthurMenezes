using BillingService.Domain.Entities;

namespace BillingService.Infrastructure.Data.Repositories.Interfaces;

public interface IInvoiceRepository
{
    public Task RegisterInvoiceAsync(Invoice invoice, CancellationToken cancellationToken);
    public Task<Invoice?> GetInvoiceByIdAsync(int id, CancellationToken cancellationToken);
    public Task AddProductToInvoiceAsync(Invoice invoice, CancellationToken cancellationToken);
    public Task UpdateProductAsync(Invoice invoice, CancellationToken cancellationToken);
}
