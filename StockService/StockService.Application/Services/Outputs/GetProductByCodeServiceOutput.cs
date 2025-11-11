namespace StockService.Application.Services.Outputs;

public sealed class GetProductByCodeServiceOutput
{
    public string Id { get; }
    public string Code { get; }
    public string Description { get; }
    public int Balance { get; }
    public DateTime CreatedAt { get; }

    private GetProductByCodeServiceOutput(string id, string code, string description, int balance, DateTime createdAt)
    {
        Id = id;
        Code = code;
        Description = description;
        Balance = balance;
        CreatedAt = createdAt;
    }

    public static GetProductByCodeServiceOutput Factory(string id, string code, string description, int balance, DateTime createdAt)
        => new GetProductByCodeServiceOutput(id, code, description, balance, createdAt);
}
