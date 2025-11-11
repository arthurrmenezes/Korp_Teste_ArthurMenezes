namespace BillingService.Application.Services.Inputs;

public sealed class AddItemsToInvoiceByIdServiceInput
{
    public AddItemsToInvoiceByIdServiceInputItems[] Items { get; }

    private AddItemsToInvoiceByIdServiceInput(AddItemsToInvoiceByIdServiceInputItems[] items)
    {
        Items = items;
    }

    public static AddItemsToInvoiceByIdServiceInput Factory(AddItemsToInvoiceByIdServiceInputItems[] items)
        => new AddItemsToInvoiceByIdServiceInput(items);
}

public sealed class AddItemsToInvoiceByIdServiceInputItems
{
    public string ProductCode { get; }
    public int Quantity { get; }

    public AddItemsToInvoiceByIdServiceInputItems(string productCode, int quantity)
    {
        ProductCode = productCode;
        Quantity = quantity;
    }
}
