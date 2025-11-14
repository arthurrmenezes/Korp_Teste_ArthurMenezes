namespace StockService.Domain.Entities;

public class Product
{
    public Guid Id { get; private set; }
    public string Code { get; private set; }
    public string Description { get; private set; }
    public int Balance { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public Product(string code, string description, int balance)
    {
        Id = Guid.NewGuid();

        ValidateDomain(code, description, balance);

        Code = code;
        Description = description;
        Balance = balance;
        CreatedAt = DateTime.UtcNow;
    }

    private void ValidateDomain(string code, string description, int balance)
    {
        if (string.IsNullOrEmpty(code))
            throw new ArgumentException("Code cannot be null or empty.");

        if (string.IsNullOrEmpty(description))
            throw new ArgumentException("Description cannot be null or empty.");

        if (balance < 0)
            throw new ArgumentException("Balance cannot be negative.");
    }

    public void IncrementBalance(int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity to increment must be greater than 0.");

        Balance = Balance + quantity;
    }

    public void DecrementBalance(int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity to decrement must be greater than 0.");

        if (Balance < quantity)
            throw new InvalidOperationException("Insufficient balance to decrement this quantity.");

        Balance = Balance - quantity;
    }
}
