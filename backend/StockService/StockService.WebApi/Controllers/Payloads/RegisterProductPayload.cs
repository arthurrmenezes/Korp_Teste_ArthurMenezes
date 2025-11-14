namespace StockService.WebApi.Controllers.Payloads;

public class RegisterProductPayload
{
    public string Code { get; init; }
    public string Description { get; init; }
    public int Balance { get; init; }

    public RegisterProductPayload(string code, string description, int balance)
    {
        Code = code;
        Description = description;
        Balance = balance;
    }
}
