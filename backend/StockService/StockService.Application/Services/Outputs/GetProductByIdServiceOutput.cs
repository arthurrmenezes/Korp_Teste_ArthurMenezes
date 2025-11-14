namespace StockService.Application.Services.Outputs;

public sealed class GetProductByIdServiceOutput
{
    public string Id { get; }
    public string Code { get; }
    public string Description { get; }
    public int Balance { get; }
    public DateTime CreatedAt { get; }

    private GetProductByIdServiceOutput(string id, string code, string description, int balance, DateTime createdAt)
    {
        Id = id;
        Code = code;
        Description = description;
        Balance = balance;
        CreatedAt = createdAt;
    }

    public static GetProductByIdServiceOutput Factory(string id, string code, string description, int balance, DateTime createdAt)
        => new GetProductByIdServiceOutput(id, code, description, balance, createdAt);
}
