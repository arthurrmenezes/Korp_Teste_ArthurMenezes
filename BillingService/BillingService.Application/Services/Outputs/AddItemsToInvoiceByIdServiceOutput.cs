namespace BillingService.Application.Services.Outputs;

public sealed class AddItemsToInvoiceByIdServiceOutput
{
    public int InvoiceId { get; }
    public string Status { get; }
    public AddItemsToInvoiceByIdServiceOutputItems[] Items { get; }
    public DateTime CreatedAt { get; }

    private AddItemsToInvoiceByIdServiceOutput(int invoiceId, string status, AddItemsToInvoiceByIdServiceOutputItems[] items, DateTime createdAt)
    {
        InvoiceId = invoiceId;
        Status = status;
        Items = items;
        CreatedAt = createdAt;
    }

    public static AddItemsToInvoiceByIdServiceOutput Factory(int id, string status, AddItemsToInvoiceByIdServiceOutputItems[] items, DateTime createdAt)
        => new AddItemsToInvoiceByIdServiceOutput(id, status, items, createdAt);
}

public sealed class AddItemsToInvoiceByIdServiceOutputItems
{
    public string Id { get; }
    public string Code { get; }
    public string Description { get; }
    public int Quantity { get; }

    public AddItemsToInvoiceByIdServiceOutputItems(string id, string code, string description, int quantity)
    {
        Id = id;
        Code = code;
        Description = description;
        Quantity = quantity;
    }
}
