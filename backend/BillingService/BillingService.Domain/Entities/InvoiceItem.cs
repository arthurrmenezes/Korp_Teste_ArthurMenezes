namespace BillingService.Domain.Entities;

public class InvoiceItem
{
    public Guid Id { get; private set; }
    public string ProductCode { get; private set; }
    public string ProductDescription { get; private set; }
    public int ProductBalance { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public InvoiceItem(string productCode, string productDescription, int productBalance)
    {
        Id = Guid.NewGuid();
        ProductCode = productCode;
        ProductDescription = productDescription;
        ProductBalance = productBalance;
        CreatedAt = DateTime.UtcNow;
    }

    public void IncrementProductBalance(int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("Balance to increment must be greater than zero.");

        ProductBalance = ProductBalance + quantity;
    }
}
