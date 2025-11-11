namespace BillingService.WebApi.Controllers.Payloads;

public sealed class AddItemsToInvoiceByIdPayload
{
    public AddItemsToInvoiceByIdPayloadItems[] Items { get; init; }

    public AddItemsToInvoiceByIdPayload(AddItemsToInvoiceByIdPayloadItems[] items)
    {
        Items = items;
    }
}

public sealed class AddItemsToInvoiceByIdPayloadItems
{
    public string ProductCode { get; init; }
    public int Quantity { get; init; }

    public AddItemsToInvoiceByIdPayloadItems(string productCode, int quantity)
    {
        ProductCode = productCode;
        Quantity = quantity;
    }
}
