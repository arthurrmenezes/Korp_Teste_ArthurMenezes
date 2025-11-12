using BillingService.Domain.Entities;
using BillingService.Infrastructure.Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Npgsql;

namespace BillingService.Infrastructure.Data.Repositories;

public class InvoiceRepository : IInvoiceRepository
{
    private readonly DataContext _dataContext;

    public InvoiceRepository(DataContext dataContext)
    {
        _dataContext = dataContext;
    }

    public async Task RegisterInvoiceAsync(Invoice invoice, CancellationToken cancellationToken)
    {
        await _dataContext.Invoices.AddAsync(invoice, cancellationToken);
        await _dataContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<Invoice?> GetInvoiceByIdAsync(int id, CancellationToken cancellationToken)
    {
        var invoice = await _dataContext.Invoices
            .Include(i => i.Items)
            .FirstOrDefaultAsync(i => i.Id == id, cancellationToken);
        return invoice;
    }

    public async Task AddProductToInvoiceAsync(Invoice invoice, CancellationToken cancellationToken)
    {
        foreach (var item in invoice.Items)
        {
            var exists = await _dataContext.InvoiceItems
                .AnyAsync(i => i.Id == item.Id, cancellationToken);

            if (!exists)
                await _dataContext.InvoiceItems.AddAsync(item, cancellationToken);
        }

        await _dataContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateInvoiceAsync(Invoice invoice, CancellationToken cancellationToken)
    {
        _dataContext.Invoices.Update(invoice);
        await _dataContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken)
    {
        return await _dataContext.Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task<Invoice?> GetInvoiceToUpdateByIdAsync(int id, CancellationToken cancellationToken)
    {
        var sql = @"SELECT * FROM ""Invoices"" WHERE ""Id"" = @id FOR UPDATE";
        return await _dataContext.Invoices
            .FromSqlRaw(sql, new NpgsqlParameter("@id", id))
            .Include(i => i.Items)
            .AsTracking()
            .FirstOrDefaultAsync(cancellationToken);
    }
}
