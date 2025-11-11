using BillingService.Application.Services.Inputs;
using BillingService.Application.Services.Outputs;

namespace BillingService.Application.Services.Interfaces;

public interface IInvoiceService
{
    public Task<CreateInvoiceServiceOutput> CreateInvoiceServiceAsync(
        CreateInvoiceServiceInput input,
        CancellationToken cancellationToken);

    public Task<GetInvoiceByIdServiceOutput> GetInvoiceByIdServiceAsync(
        int invoiceId,
        CancellationToken cancellationToken);

    public Task<AddItemsToInvoiceByIdServiceOutput> AddItemsToInvoiceByIdServiceAsync(
        int invoiceId,
        AddItemsToInvoiceByIdServiceInput input,
        CancellationToken cancellationToken);

    public Task<PrintInvoiceByIdServiceOutput> PrintInvoiceByIdServiceAsync(
        int invoiceId,
        CancellationToken cancellationToken);
}
