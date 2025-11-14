using BillingService.Application.Services.InvoiceService.Inputs;
using BillingService.Application.Services.InvoiceService.Interfaces;
using BillingService.WebApi.Controllers.InvoiceController.Payloads;
using Microsoft.AspNetCore.Mvc;

namespace BillingService.WebApi.Controllers.InvoiceController;

[ApiController]
[Route("api/v1/invoices")]
public class InvoiceController : ControllerBase
{
    private readonly IInvoiceService _invoiceService;

    public InvoiceController(IInvoiceService invoiceService)
    {
        _invoiceService = invoiceService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateInvoiceAsync(
        [FromBody] CreateInvoicePayload input,
        CancellationToken cancellationToken)
    {
        if (input is null || input.Items.Length == 0)
            return BadRequest("The invoice item cannot be null.");

        if (input.Items.Any(p => string.IsNullOrWhiteSpace(p.ProductCode)))
            return BadRequest("The product code cannot be null or whitespace.");

        if (input.Items.Any(p => p.Quantity <= 0))
            return BadRequest("All products in the list must have a quantity greater than zero.");

        var items = input.Items.Select(item => new CreateInvoiceServiceInputItems(
                productCode: item.ProductCode,
                quantity: item.Quantity))
            .ToArray();

        var response = await _invoiceService.CreateInvoiceServiceAsync(
            input: CreateInvoiceServiceInput.Factory(items), 
            cancellationToken: cancellationToken);

        return CreatedAtAction("GetInvoiceById", new { invoiceId = response.InvoiceId }, response);
    }

    [HttpGet]
    [Route("{invoiceId}")]
    public async Task<IActionResult> GetInvoiceByIdAsync(
        [FromRoute] int invoiceId,
        CancellationToken cancellationToken)
    {
        if (invoiceId <= 0 || invoiceId > int.MaxValue)
            return BadRequest($"Invalid invoice ID. Remember: the invoice ID must be greater than zero and less than {int.MaxValue}.");

        var response = await _invoiceService.GetInvoiceByIdServiceAsync(
            invoiceId: invoiceId, 
            cancellationToken: cancellationToken);

        return Ok(response);
    }

    [HttpPost]
    [Route("{invoiceId}/add-product")]
    public async Task<IActionResult> AddItemsToInvoiceByIdAsync(
        [FromRoute] int invoiceId,
        [FromBody] AddItemsToInvoiceByIdPayload input,
        CancellationToken cancellationToken)
    {
        if (invoiceId <= 0 || invoiceId > int.MaxValue)
            return BadRequest($"Invalid invoice ID. Remember: the invoice ID must be greater than zero and less than {int.MaxValue}.");

        if (input is null || input.Items.Length == 0)
            return BadRequest("Add at least one product to the invoice.");

        if (input.Items.Any(p => string.IsNullOrWhiteSpace(p.ProductCode)))
            return BadRequest("The product code cannot be null or whitespace.");

        if (input.Items.Any(p => p.Quantity <= 0))
            return BadRequest("All products in the list must have a quantity greater than zero.");

        var items = input.Items.Select(i => new AddItemsToInvoiceByIdServiceInputItems(
            productCode: i.ProductCode,
            quantity: i.Quantity)).ToArray();

        var response = await _invoiceService.AddItemsToInvoiceByIdServiceAsync(
            invoiceId: invoiceId,
            input: AddItemsToInvoiceByIdServiceInput.Factory(items),
            cancellationToken: cancellationToken);

        return Ok(response);
    }

    [HttpPost]
    [Route("{invoiceId}/print")]
    public async Task<IActionResult> PrintInvoiceByIdAsync(
        int invoiceId,
        CancellationToken cancellationToken)
    {
        if (invoiceId <= 0 || invoiceId > int.MaxValue)
            return BadRequest($"Invalid invoice ID. Remember: the invoice ID must be greater than zero and less than {int.MaxValue}.");

        var response = await _invoiceService.PrintInvoiceByIdServiceAsync(invoiceId, cancellationToken);

        return Ok(response);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllInvoicesAsync(
        CancellationToken cancellationToken,
        int pageNumber = 1,
        int pageSize = 5)
    {
        if (pageNumber <= 0)
            pageNumber = 1;

        if (pageSize <= 0)
            pageSize = 5;

        var response = await _invoiceService.GetAllInvoicesServiceAsync(
            pageNumber: pageNumber,
            pageSize: pageSize,
            cancellationToken: cancellationToken);

        return Ok(response);
    }
}
