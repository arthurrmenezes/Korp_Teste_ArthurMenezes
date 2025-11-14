using BillingService.Application.Services.InvoiceService.Inputs;
using BillingService.Application.Services.InvoiceService.Interfaces;
using BillingService.Application.Services.InvoiceService.Outputs;
using BillingService.Domain.Entities;
using BillingService.Domain.ENUMs;
using BillingService.Infrastructure.Data.Repositories.Interfaces;
using BillingService.Infrastructure.Services.External.StockService.Inputs;
using BillingService.Infrastructure.Services.External.StockService.Interfaces;

namespace BillingService.Application.Services.InvoiceService;

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
                throw new KeyNotFoundException($"Product with code {item.ProductCode} was not found.");

            if (product.ProductBalance < item.Quantity)
                throw new InvalidOperationException($"Insufficient stock for product '{product.ProductDescription}'. Requested: {item.Quantity}, Available: {product.ProductBalance}");

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
            throw new KeyNotFoundException($"Invoice with id {invoiceId} was not found.");

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
        await using var transaction = await _invoiceRepository.BeginTransactionAsync(cancellationToken);

        try
        {
            var invoice = await _invoiceRepository.GetInvoiceToUpdateByIdAsync(invoiceId, cancellationToken);
            if (invoice is null)
                throw new KeyNotFoundException($"Invoice with id {invoiceId} was not found.");

            if (invoice.Status == InvoiceStatus.Closed)
                throw new InvalidOperationException("Cannot add products to a closed invoice.");

            foreach (var item in input.Items)
            {
                var product = await _stockService.GetProductByCodeServiceAsync(item.ProductCode, cancellationToken);
                if (product is null)
                    throw new KeyNotFoundException($"Product with code {item.ProductCode} was not found.");

                var totalProductBalance = invoice.Items.FirstOrDefault(i => i.ProductCode == item.ProductCode)?.ProductBalance ?? 0;

                if (product.ProductBalance < (totalProductBalance + item.Quantity))
                    throw new InvalidOperationException($"Insufficient stock for product '{product.ProductDescription}'. Requested: {item.Quantity}, Available: {product.ProductBalance}");

                var invoiceItem = new InvoiceItem(
                    productCode: item.ProductCode,
                    productDescription: product.ProductDescription,
                    productBalance: item.Quantity);

                invoice.AddProductToInvoiceList(invoiceItem);
            }
            await _invoiceRepository.AddProductToInvoiceAsync(invoice, cancellationToken);
            await transaction.CommitAsync(cancellationToken);

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
        catch (Exception)
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    public async Task<PrintInvoiceByIdServiceOutput> PrintInvoiceByIdServiceAsync(int invoiceId, CancellationToken cancellationToken)
    {
        await using var transaction = await _invoiceRepository.BeginTransactionAsync(cancellationToken);
       
        try
        {
            var invoice = await _invoiceRepository.GetInvoiceToUpdateByIdAsync(invoiceId, cancellationToken);
            if (invoice is null)
                throw new KeyNotFoundException($"Invoice with id {invoiceId} was not found.");

            if (invoice.Status != InvoiceStatus.Open)
                throw new InvalidOperationException($"Only invoices with the status {InvoiceStatus.Open} can be printed");

            var items = invoice.Items.Select(i => new DeductBalanceByProductListServiceInputProduct(
                code: i.ProductCode,
                quantity: i.ProductBalance)).ToArray();

            await _stockService.DeductBalanceByProductListServiceAsync(DeductBalanceByProductListServiceInput.Factory(items), cancellationToken);

            invoice.CloseInvoice();

            await _invoiceRepository.UpdateInvoiceAsync(invoice, cancellationToken);

            await transaction.CommitAsync(cancellationToken);

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
        catch (Exception)
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    public async Task<GetAllInvoicesServiceOutput> GetAllInvoicesServiceAsync(int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        var invoices = await _invoiceRepository.GetAllInvoicesAsync(pageNumber, pageSize, cancellationToken);

        var totalInvoicesCount = await _invoiceRepository.GetTotalInvoicesCountAsync(cancellationToken);

        var invoiceListOutput = invoices.Select(i => new GetAllInvoicesServiceOutputInvoice(
            invoiceId: i.Id,
            status: i.Status.ToString(),
            createdAt: i.CreatedAt,
            totalProducts: i.Items.Count,
            products: i.Items.Select(p => new GetAllInvoicesServiceOutputInvoiceProductList(
                productId: p.Id.ToString(),
                code: p.ProductCode,
                description: p.ProductDescription,
                quantity: p.ProductBalance)).ToArray())).ToArray();

        var output = GetAllInvoicesServiceOutput.Factory(
            totalInvoices: totalInvoicesCount,
            pageNumber: pageNumber,
            pageSize: pageSize,
            totalPages: (int)Math.Ceiling((double) totalInvoicesCount / pageSize),
            invoices: invoiceListOutput);

        return output;
    }
}
