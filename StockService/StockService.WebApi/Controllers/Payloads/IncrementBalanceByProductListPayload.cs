namespace StockService.WebApi.Controllers.Payloads;

public class IncrementBalanceByProductListPayload
{
    public IncrementBalanceByProductListPayloadProduct[] ProductList { get; init; }

    public IncrementBalanceByProductListPayload(IncrementBalanceByProductListPayloadProduct[] productList)
    {
        ProductList = productList;
    }
}

public sealed class IncrementBalanceByProductListPayloadProduct
{
    public string Code { get; init; }
    public int Quantity { get; init; }

    public IncrementBalanceByProductListPayloadProduct(string code, int quantity)
    {
        Code = code;
        Quantity = quantity;
    }
}