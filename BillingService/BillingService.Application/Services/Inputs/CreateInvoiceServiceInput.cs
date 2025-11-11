namespace BillingService.Application.Services.Inputs;

public sealed class CreateInvoiceServiceInput
{
    public CreateInvoiceServiceInputItems[] Items { get; }

    private CreateInvoiceServiceInput(CreateInvoiceServiceInputItems[] items)
    {
        Items = items;
    }

    public static CreateInvoiceServiceInput Factory(CreateInvoiceServiceInputItems[] items)
        => new CreateInvoiceServiceInput(items);
}

public sealed class CreateInvoiceServiceInputItems
{
    public string ProductCode { get; }
    public int Quantity { get; }

    public CreateInvoiceServiceInputItems(string productCode, int quantity)
    {
        ProductCode = productCode;
        Quantity = quantity;
    }
}
