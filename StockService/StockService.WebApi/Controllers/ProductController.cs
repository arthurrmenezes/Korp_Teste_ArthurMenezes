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
        var response = await _productService.RegisterProductServiceAsync(
            input: RegisterProductServiceInput.Factory(
                code: input.Code,
                description: input.Description,
                balance: input.Balance),
            cancellationToken: cancellationToken);

        if (response is null)
            return BadRequest();

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
        if (response is null)
            return NotFound();

        return Ok(response);
    }

    [HttpGet]
    [Route("code/{code}")]
    public async Task<IActionResult> GetProductByCode(
        [FromRoute] string code,
        CancellationToken cancellationToken)
    {
        var response = await _productService.GetProductByCodeServiceAsync(code, cancellationToken);
        if (response is null)
            return NotFound();

        return Ok(response);
    }

    [HttpPost]
    [Route("{code}/increment-balance")]
    public async Task<IActionResult> IncrementBalanceByProductCodeAsync(
        [FromRoute] string code,
        [FromBody] IncrementBalanceByProductCodePayload input,
        CancellationToken cancellationToken)
    {
        if (input.Quantity <= 0)
            return BadRequest("The quantity needs to be greater than 0.");

        await _productService.IncrementBalanceByProductCodeServiceAsync(
            code: code,
            quantity: input.Quantity,
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

        var productList = input.ProductList.Select(p => new DeductBalanceByProductListServiceInputProduct(
            code: p.Code,
            quantity: p.Quantity)).ToArray();

        await _productService.DeductBalanceByProductListServiceAsync(
            input: DeductBalanceByProductListServiceInput.Factory(productList),
            cancellationToken: cancellationToken);

        return NoContent();
    }
}
