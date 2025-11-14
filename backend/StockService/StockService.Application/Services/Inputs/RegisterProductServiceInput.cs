namespace StockService.Application.Services.Inputs;

public sealed class RegisterProductServiceInput
{
    public string Code { get; }
    public string Description { get; }
    public int Balance { get; }

    private RegisterProductServiceInput(string code, string description, int balance)
    {
        Code = code;
        Description = description;
        Balance = balance;
    }

    public static RegisterProductServiceInput Factory(string code, string description, int balance)
        => new RegisterProductServiceInput(code, description, balance);
}

