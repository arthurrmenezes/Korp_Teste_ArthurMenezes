namespace BillingService.Application.Services.Outputs;

public sealed class PrintInvoiceByIdServiceOutput
{
    public int InvoiceId { get; }
    public string Status { get; }
    public PrintInvoiceByIdServiceOutputItems[] Items { get; }
    public DateTime CreatedAt { get; }

    private PrintInvoiceByIdServiceOutput(int invoiceId, string status, PrintInvoiceByIdServiceOutputItems[] items, DateTime createdAt)
    {
        InvoiceId = invoiceId;
        Status = status;
        Items = items;
        CreatedAt = createdAt;
    }

    public static PrintInvoiceByIdServiceOutput Factory(int invoiceId, string status, PrintInvoiceByIdServiceOutputItems[] items, DateTime createdAt)
        => new PrintInvoiceByIdServiceOutput(invoiceId, status, items, createdAt);
}

public sealed class PrintInvoiceByIdServiceOutputItems
{
    public string Id { get; }
    public string Code { get; }
    public string Description { get; }
    public int Quantity { get; }

    public PrintInvoiceByIdServiceOutputItems(string id, string code, string description, int quantity)
    {
        Id = id;
        Code = code;
        Description = description;
        Quantity = quantity;
    }
}
