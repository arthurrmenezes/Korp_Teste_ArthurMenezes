namespace BillingService.Application.Services.InvoiceService.Outputs;

public sealed class GetAllInvoicesServiceOutput
{
    public int TotalInvoices { get; }
    public int PageNumber { get; }
    public int PageSize { get; }
    public int TotalPages { get; }
    public GetAllInvoicesServiceOutputInvoice[] Invoices { get; }

    private GetAllInvoicesServiceOutput(int totalInvoices, int pageNumber, int pageSize, int totalPages, GetAllInvoicesServiceOutputInvoice[] invoices)
    {
        TotalInvoices = totalInvoices;
        PageNumber = pageNumber;
        PageSize = pageSize;
        TotalPages = totalPages;
        Invoices = invoices;
    }

    public static GetAllInvoicesServiceOutput Factory(int totalInvoices, int pageNumber, int pageSize, int totalPages, GetAllInvoicesServiceOutputInvoice[] invoices)
        => new GetAllInvoicesServiceOutput(totalInvoices, pageNumber, pageSize, totalPages, invoices);
}

public sealed class GetAllInvoicesServiceOutputInvoice
{
    public int InvoiceId { get; }
    public string Status { get; }
    public DateTime CreatedAt { get; }
    public int TotalProducts { get; }
    public GetAllInvoicesServiceOutputInvoiceProductList[] Products { get; }

    public GetAllInvoicesServiceOutputInvoice(int invoiceId, string status, DateTime createdAt, int totalProducts, 
        GetAllInvoicesServiceOutputInvoiceProductList[] products)
    {
        InvoiceId = invoiceId;
        Status = status;
        CreatedAt = createdAt;
        TotalProducts = totalProducts;
        Products = products;
    }
}

public sealed class GetAllInvoicesServiceOutputInvoiceProductList
{
    public string ProductId { get; }
    public string Code { get; }
    public string Description { get; }
    public int Quantity { get; }

    public GetAllInvoicesServiceOutputInvoiceProductList(string productId, string code, string description, int quantity)
    {
        ProductId = productId;
        Code = code;
        Description = description;
        Quantity = quantity;
    }
}
