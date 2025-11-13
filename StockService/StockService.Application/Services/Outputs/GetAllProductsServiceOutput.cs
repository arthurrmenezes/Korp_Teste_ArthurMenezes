namespace StockService.Application.Services.Outputs;

public sealed class GetAllProductsServiceOutput
{
    public int TotalProducts { get; }
    public int PageNumber { get; }
    public int PageSize { get; }
    public int TotalPages { get; }
    public GetAllProductsServiceOutputProduct[] Product { get; }

    private GetAllProductsServiceOutput(int totalProducts, int pageNumber, int pageSize, int totalPages, GetAllProductsServiceOutputProduct[] product)
    {
        TotalProducts = totalProducts;
        PageNumber = pageNumber;
        PageSize = pageSize;
        TotalPages = totalPages;
        Product = product;
    }

    public static GetAllProductsServiceOutput Factory(int totalProducts, int pageNumber, int pageSize, int totalPages, GetAllProductsServiceOutputProduct[] product)
        => new GetAllProductsServiceOutput(totalProducts, pageNumber, pageSize, totalPages, product);
}

public sealed class GetAllProductsServiceOutputProduct
{
    public string Id { get; }
    public string Code { get; }
    public string Description { get; }
    public int Balance { get; }
    public DateTime CreatedAt { get; }

    public GetAllProductsServiceOutputProduct(string id, string code, string description, int balance, DateTime createdAt)
    {
        Id = id;
        Code = code;
        Description = description;
        Balance = balance;
        CreatedAt = createdAt;
    }
}
