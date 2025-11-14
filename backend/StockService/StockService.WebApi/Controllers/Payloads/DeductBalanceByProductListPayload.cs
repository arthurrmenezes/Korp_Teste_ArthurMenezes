namespace StockService.WebApi.Controllers.Payloads;

public sealed class DeductBalanceByProductListPayload
{
    public DeductBalanceByProductListPayloadProduct[] ProductList { get; init; }

    public DeductBalanceByProductListPayload(DeductBalanceByProductListPayloadProduct[] productList)
    {
        ProductList = productList;
    }
}

public sealed class DeductBalanceByProductListPayloadProduct
{
    public string Code { get; }
    public int Quantity { get; }

    public DeductBalanceByProductListPayloadProduct(string code, int quantity)
    {
        Code = code;
        Quantity = quantity;
    }
}
