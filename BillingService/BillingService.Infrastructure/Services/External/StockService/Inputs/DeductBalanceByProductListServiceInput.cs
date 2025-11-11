namespace BillingService.Infrastructure.Services.External.StockService.Inputs;

public sealed class DeductBalanceByProductListServiceInput
{
    public DeductBalanceByProductListServiceInputProduct[] ProductList { get; }

    private DeductBalanceByProductListServiceInput(DeductBalanceByProductListServiceInputProduct[] productList)
    {
        ProductList = productList;
    }

    public static DeductBalanceByProductListServiceInput Factory(DeductBalanceByProductListServiceInputProduct[] productList)
        => new DeductBalanceByProductListServiceInput(productList);
}

public sealed class DeductBalanceByProductListServiceInputProduct
{
    public string Code { get; }
    public int Quantity { get; }

    public DeductBalanceByProductListServiceInputProduct(string code, int quantity)
    {
        Code = code;
        Quantity = quantity;
    }
}
