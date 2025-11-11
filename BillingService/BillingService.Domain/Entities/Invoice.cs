using BillingService.Domain.ENUMs;

namespace BillingService.Domain.Entities;

public class Invoice
{
    public int Id { get; private set; }
    public InvoiceStatus Status { get; private set; }
    public List<InvoiceItem> Items { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public Invoice()
    {
        Status = InvoiceStatus.Open;
        Items = new List<InvoiceItem>();
        CreatedAt = DateTime.UtcNow;
    }

    public void AddProductToInvoiceList(InvoiceItem item)
    {
        if (Status != InvoiceStatus.Open)
            throw new ArgumentException("Cannot add products to a closed invoice.");

        var existingItem = Items.FirstOrDefault(p => p.ProductCode == item.ProductCode);
        if (existingItem != null)
        {
            existingItem.IncrementProductBalance(item.ProductBalance);
            return;
        }

        Items.Add(item);
    }

    public void CloseInvoice()
    {
        if (Status == InvoiceStatus.Closed)
            throw new ArgumentException("Invoice is already closed.");

        if (Items.Count == 0)
            throw new ArgumentException("Cannot close an invoice with no items.");

        Status = InvoiceStatus.Closed;
    }
}
