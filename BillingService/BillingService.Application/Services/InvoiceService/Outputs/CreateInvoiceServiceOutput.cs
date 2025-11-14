namespace BillingService.Application.Services.InvoiceService.Outputs;

public sealed class CreateInvoiceServiceOutput
{
    public int InvoiceId { get; }
    public string Status { get; }
    public CreateInvoiceServiceOutputInvoiceItems[] Items { get; }
    public DateTime CreatedAt { get; }

    private CreateInvoiceServiceOutput(int invoiceId, string status, CreateInvoiceServiceOutputInvoiceItems[] items, DateTime createdAt)
    {
        InvoiceId = invoiceId;
        Status = status;
        Items = items;
        CreatedAt = createdAt;
    }

    public static CreateInvoiceServiceOutput Factory(int id, string status, CreateInvoiceServiceOutputInvoiceItems[] items, DateTime createdAt)
        => new CreateInvoiceServiceOutput(id, status, items, createdAt);
}

public sealed class CreateInvoiceServiceOutputInvoiceItems
{
    public string Id { get; }
    public string Code { get; }
    public string Description { get; }
    public int Quantity { get; }

    public CreateInvoiceServiceOutputInvoiceItems(string id, string code, string description, int quantity)
    {
        Id = id;
        Code = code;
        Description = description;
        Quantity = quantity;
    }
}
