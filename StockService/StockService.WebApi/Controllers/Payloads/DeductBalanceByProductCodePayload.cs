namespace StockService.WebApi.Controllers.Payloads;

public class DeductBalanceByProductCodePayload
{
    public int Quantity { get; init; }

    public DeductBalanceByProductCodePayload(int quantity)
    {
        Quantity = quantity;
    }
}
