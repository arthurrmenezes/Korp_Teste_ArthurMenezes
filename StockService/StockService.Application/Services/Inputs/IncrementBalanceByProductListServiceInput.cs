namespace StockService.Application.Services.Inputs;

public sealed class IncrementBalanceByProductListServiceInput
{
    public IncrementBalanceByProductListServiceInputProduct[] ProductList { get; }

    private IncrementBalanceByProductListServiceInput(IncrementBalanceByProductListServiceInputProduct[] productList)
    {
        ProductList = productList;
    }

    public static IncrementBalanceByProductListServiceInput Factory(IncrementBalanceByProductListServiceInputProduct[] productList)
        => new IncrementBalanceByProductListServiceInput(productList);
}

public sealed class IncrementBalanceByProductListServiceInputProduct
{
    public string Code { get; }
    public int Quantity { get; }

    public IncrementBalanceByProductListServiceInputProduct(string code, int quantity)
    {
        Code = code;
        Quantity = quantity;
    }
}
