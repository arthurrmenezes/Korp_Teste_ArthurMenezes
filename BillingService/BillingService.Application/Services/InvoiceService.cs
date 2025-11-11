using BillingService.Application.Services.Inputs;
using BillingService.Application.Services.Interfaces;
using BillingService.Application.Services.Outputs;
using BillingService.Domain.Entities;
using BillingService.Domain.ENUMs;
using BillingService.Infrastructure.Data.Repositories.Interfaces;
using BillingService.Infrastructure.Services.External.StockService.Inputs;
using BillingService.Infrastructure.Services.External.StockService.Interfaces;

namespace BillingService.Application.Services;

public sealed class InvoiceService : IInvoiceService
{
    private IInvoiceRepository _invoiceRepository;
    private IStockService _stockService;

    public InvoiceService(IInvoiceRepository invoiceRepository, IStockService stockService)
    {
        _invoiceRepository = invoiceRepository;
        _stockService = stockService;
    }

    public async Task<CreateInvoiceServiceOutput> CreateInvoiceServiceAsync(CreateInvoiceServiceInput input, CancellationToken cancellationToken)
    {
        var invoice = new Invoice();

        foreach (var item in input.Items)
        {
            var product = await _stockService.GetProductByCodeServiceAsync(item.ProductCode, cancellationToken);
            if (product is null)
                throw new ArgumentException($"Product with code {item.ProductCode} was not found.");

            if (product.ProductBalance < item.Quantity)
                throw new ArgumentException($"Insufficient stock for product '{product.ProductDescription}'. Requested: {item.Quantity}, Available: {product.ProductBalance}");

            var invoiceItem = new InvoiceItem(
                productCode: item.ProductCode,
                productDescription: product.ProductDescription,
                productBalance: item.Quantity);

            invoice.AddProductToInvoiceList(invoiceItem);
        }

        await _invoiceRepository.RegisterInvoiceAsync(invoice, cancellationToken);

        var outputItems = invoice.Items.Select(i => new CreateInvoiceServiceOutputInvoiceItems(
            id: i.Id.ToString(),
            code: i.ProductCode,
            description: i.ProductDescription,
            quantity: i.ProductBalance)).ToArray();

        var output = CreateInvoiceServiceOutput.Factory(
            id: invoice.Id,
            status: invoice.Status.ToString(),
            items: outputItems,
            createdAt: invoice.CreatedAt);

        return output;
    }

    public async Task<GetInvoiceByIdServiceOutput> GetInvoiceByIdServiceAsync(int invoiceId, CancellationToken cancellationToken)
    {
        var invoice = await _invoiceRepository.GetInvoiceByIdAsync(invoiceId, cancellationToken);
        if (invoice is null)
            throw new ArgumentException($"Invoice with id {invoiceId} was not found.");

        var outputItems = invoice.Items.Select(i => new GetInvoiceByIdServiceOutputItems(
            id: i.Id.ToString(),
            code: i.ProductCode,
            description: i.ProductDescription,
            quantity: i.ProductBalance)).ToArray();

        var output = GetInvoiceByIdServiceOutput.Factory(
            invoiceId: invoice.Id,
            status: invoice.Status.ToString(),
            items: outputItems,
            createdAt: invoice.CreatedAt);

        return output;
    }

    public async Task<AddItemsToInvoiceByIdServiceOutput> AddItemsToInvoiceByIdServiceAsync(int invoiceId, AddItemsToInvoiceByIdServiceInput input, CancellationToken cancellationToken)
    {
        var invoice = await _invoiceRepository.GetInvoiceByIdAsync(invoiceId, cancellationToken);
        if (invoice is null)
            throw new ArgumentException($"Invoice with id {invoiceId} was not found.");

        if (invoice.Status == InvoiceStatus.Closed)
            throw new ArgumentException("Cannot add products to a closed invoice.");

        foreach (var item in input.Items)
        {
            var product = await _stockService.GetProductByCodeServiceAsync(item.ProductCode, cancellationToken);
            if (product is null)
                throw new ArgumentException($"Product with code {item.ProductCode} was not found.");

            var totalProductBalance = invoice.Items.FirstOrDefault(i => i.ProductCode == item.ProductCode)?.ProductBalance ?? 0;

            if (product.ProductBalance < (totalProductBalance + item.Quantity))
                throw new ArgumentException($"Insufficient stock for product '{product.ProductDescription}'. Requested: {item.Quantity}, Available: {product.ProductBalance}");

            var invoiceItem = new InvoiceItem(
                productCode: item.ProductCode,
                productDescription: product.ProductDescription,
                productBalance: item.Quantity);

            invoice.AddProductToInvoiceList(invoiceItem);
        }

        await _invoiceRepository.AddProductToInvoiceAsync(invoice, cancellationToken);

        var outputItems = invoice.Items.Select(i => new AddItemsToInvoiceByIdServiceOutputItems(
            id: i.Id.ToString(),
            code: i.ProductCode,
            description: i.ProductDescription,
            quantity: i.ProductBalance)).ToArray();

        var output = AddItemsToInvoiceByIdServiceOutput.Factory(
            id: invoice.Id,
            status: invoice.Status.ToString(),
            items: outputItems,
            createdAt: invoice.CreatedAt);

        return output;
    }

    public async Task<PrintInvoiceByIdServiceOutput> PrintInvoiceByIdServiceAsync(int invoiceId, CancellationToken cancellationToken)
    {
        var invoice = await _invoiceRepository.GetInvoiceByIdAsync(invoiceId, cancellationToken);
        if (invoice is null)
            throw new ArgumentException($"Invoice with id {invoiceId} was not found.");

        if (invoice.Status != InvoiceStatus.Open)
            throw new ArgumentException($"Only invoices with the status {InvoiceStatus.Open} can be printed");

        var items = invoice.Items.Select(i => new DeductBalanceByProductListServiceInputProduct(
            code: i.ProductCode,
            quantity: i.ProductBalance)).ToArray();

        try
        {
            await _stockService.DeductBalanceByProductListServiceAsync(DeductBalanceByProductListServiceInput.Factory(items), cancellationToken);

            invoice.CloseInvoice();

            await _invoiceRepository.UpdateProductAsync(invoice, cancellationToken);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to communicate with the stock service. The stock was not deducted and the invoice remained open.", ex);
        }

        var outputItems = invoice.Items.Select(i => new PrintInvoiceByIdServiceOutputItems(
            id: i.Id.ToString(),
            code: i.ProductCode,
            description: i.ProductDescription,
            quantity: i.ProductBalance)).ToArray();

        var output = PrintInvoiceByIdServiceOutput.Factory(
            invoiceId: invoice.Id,
            status: invoice.Status.ToString(),
            items: outputItems,
            createdAt: invoice.CreatedAt);

        return output;
    }
}
