using System.ComponentModel.DataAnnotations;

namespace StockService.WebApi.Controllers.Payloads;

public class RegisterProductPayload
{
    [Required]
    public string Code { get; init; }
    [Required]
    public string Description { get; init; }
    [Required]
    [Range(0, int.MaxValue)]
    public int Balance { get; init; }

    public RegisterProductPayload(string code, string description, int balance)
    {
        Code = code;
        Description = description;
        Balance = balance;
    }
}
