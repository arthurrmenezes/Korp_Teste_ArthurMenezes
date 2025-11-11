namespace StockService.WebApi.Controllers.Payloads;

public class IncrementBalanceByProductCodePayload
{
    public int Quantity { get; init; }

    public IncrementBalanceByProductCodePayload(int quantity)
    {
        Quantity = quantity;
    }
}
