using Microsoft.AspNetCore.Mvc;
using StockService.Application.Services.Inputs;
using StockService.Application.Services.Interfaces;
using StockService.WebApi.Controllers.Payloads;

namespace StockService.WebApi.Controllers;

[ApiController]
[Route("api/v1/products")]
public class ProductController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpPost]
    public async Task<IActionResult> RegisterProductAsync(
        [FromBody] RegisterProductPayload input,
        CancellationToken cancellationToken)
    {
        if (input.Balance < 0 || input.Balance > int.MaxValue)
            return BadRequest($"Product balance must be greater than or equal to 0 and less than {int.MaxValue}.");

        var response = await _productService.RegisterProductServiceAsync(
            input: RegisterProductServiceInput.Factory(
                code: input.Code,
                description: input.Description,
                balance: input.Balance),
            cancellationToken: cancellationToken);

        return CreatedAtAction("GetProductById", new { productId = response.Id }, response);
    }

    [HttpGet]
    [Route("{productId}")]
    public async Task<IActionResult> GetProductByIdAsync(
        [FromRoute] string productId,
        CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(productId, out var guid))
            return BadRequest("The provided ID is not a valid GUID.");

        var response = await _productService.GetProductByIdServiceAsync(guid, cancellationToken);

        return Ok(response);
    }

    [HttpGet]
    [Route("code/{code}")]
    public async Task<IActionResult> GetProductByCodeAsync(
        [FromRoute] string code,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(code))
            return BadRequest("The product code cannot be null or whitespace.");

        var response = await _productService.GetProductByCodeServiceAsync(code, cancellationToken);

        return Ok(response);
    }

    [HttpPost]
    [Route("increment-balance")]
    public async Task<IActionResult> IncrementBalanceByProductListAsync(
        [FromBody] IncrementBalanceByProductListPayload input,
        CancellationToken cancellationToken)
    {
        if (input.ProductList == null || input.ProductList.Length == 0)
            return BadRequest("At least one product must be provided.");

        if (input.ProductList.Any(p => string.IsNullOrWhiteSpace(p.Code)))
            return BadRequest("The product code cannot be null or whitespace.");

        if (input.ProductList.Any(p => p.Quantity <= 0))
            return BadRequest("All products in the list must have a quantity greater than zero.");

        var productList = input.ProductList.Select(p => new IncrementBalanceByProductListServiceInputProduct(
            code: p.Code,
            quantity: p.Quantity)).ToArray();

        await _productService.IncrementBalanceByProductListServiceAsync(
            input: IncrementBalanceByProductListServiceInput.Factory(productList),
            cancellationToken: cancellationToken);

        return NoContent();
    }

    [HttpPost]
    [Route("deduct-balance")]
    public async Task<IActionResult> DeductBalanceByProductListAsync(
        [FromBody] DeductBalanceByProductListPayload input,
        CancellationToken cancellationToken)
    {
        if (input.ProductList == null || input.ProductList.Length == 0)
            return BadRequest("At least one product must be provided.");

        if (input.ProductList.Any(p => string.IsNullOrWhiteSpace(p.Code)))
            return BadRequest("The product code cannot be null or whitespace.");

        if (input.ProductList.Any(p => p.Quantity <= 0))
            return BadRequest("All products in the list must have a quantity greater than zero.");

        var productList = input.ProductList.Select(p => new DeductBalanceByProductListServiceInputProduct(
            code: p.Code,
            quantity: p.Quantity)).ToArray();

        await _productService.DeductBalanceByProductListServiceAsync(
            input: DeductBalanceByProductListServiceInput.Factory(productList),
            cancellationToken: cancellationToken);

        return NoContent();
    }

    [HttpGet]
    public async Task<IActionResult> GetAllProductsAsync(
        CancellationToken cancellationToken,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 5)
    {
        if (pageNumber <= 0)
            pageNumber = 1;

        if (pageSize <= 0)
            pageSize = 5;

        var response = await _productService.GetAllProductsServiceAsync(
            pageNumber: pageNumber, 
            pageSize: pageSize, 
            cancellationToken: cancellationToken);

        return Ok(response);
    }
}
