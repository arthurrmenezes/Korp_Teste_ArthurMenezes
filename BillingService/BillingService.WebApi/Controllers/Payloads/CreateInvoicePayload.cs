namespace BillingService.WebApi.Controllers.Payloads;

public sealed class CreateInvoicePayload
{
    public CreateInvoicePayloadInvoiceItems[] Items { get; init; }

    public CreateInvoicePayload(CreateInvoicePayloadInvoiceItems[] items)
    {
        Items = items;
    }
}

public sealed class CreateInvoicePayloadInvoiceItems
{
    public string ProductCode { get; }
    public int Quantity { get; }

    public CreateInvoicePayloadInvoiceItems(string productCode, int quantity)
    {
        ProductCode = productCode;
        Quantity = quantity;
    }
}
