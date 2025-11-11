namespace BillingService.Application.Services.Outputs;

public sealed class GetInvoiceByIdServiceOutput
{
    public int InvoiceId { get; }
    public string Status { get; }
    public GetInvoiceByIdServiceOutputItems[] Items { get; }
    public DateTime CreatedAt { get; }

    private GetInvoiceByIdServiceOutput(int invoiceId, string status, GetInvoiceByIdServiceOutputItems[] items, DateTime createdAt)
    {
        InvoiceId = invoiceId;
        Status = status;
        Items = items;
        CreatedAt = createdAt;
    }

    public static GetInvoiceByIdServiceOutput Factory(int invoiceId, string status, GetInvoiceByIdServiceOutputItems[] items, DateTime createdAt)
        => new GetInvoiceByIdServiceOutput(invoiceId, status, items, createdAt);
}

public sealed class GetInvoiceByIdServiceOutputItems
{
    public string Id { get; }
    public string Code { get; }
    public string Description { get; }
    public int Quantity { get; }

    public GetInvoiceByIdServiceOutputItems(string id, string code, string description, int quantity)
    {
        Id = id;
        Code = code;
        Description = description;
        Quantity = quantity;
    }
}
