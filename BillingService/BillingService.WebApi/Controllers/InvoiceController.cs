using BillingService.Application.Services.Inputs;
using BillingService.Application.Services.Interfaces;
using BillingService.WebApi.Controllers.Payloads;
using Microsoft.AspNetCore.Mvc;

namespace BillingService.WebApi.Controllers;

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
        if (input is null)
            return BadRequest("The invoice item cannot be null.");

        var items = input.Items.Select(item => new CreateInvoiceServiceInputItems(
                productCode: item.ProductCode,
                quantity: item.Quantity))
            .ToArray();

        var response = await _invoiceService.CreateInvoiceServiceAsync(
            input: CreateInvoiceServiceInput.Factory(items), 
            cancellationToken: cancellationToken);

        return Ok(response);
    }

    [HttpGet]
    [Route("{invoiceId}")]
    public async Task<IActionResult> GetInvoiceByIdAsync(
        [FromRoute] int invoiceId,
        CancellationToken cancellationToken)
    {
        if (invoiceId <= 0)
            return BadRequest("The invoice id must be greater than zero.");
        if (invoiceId > int.MaxValue)
            return BadRequest($"The invoice id must be lower than {int.MaxValue}.");

        var response = await _invoiceService.GetInvoiceByIdServiceAsync(
            invoiceId: invoiceId, 
            cancellationToken: cancellationToken);

        if (response is null)
            return NotFound($"Invoice with id {invoiceId} was not found.");

        return Ok(response);
    }

    [HttpPost]
    [Route("{invoiceId}/add-product")]
    public async Task<IActionResult> AddItemsToInvoiceById(
        [FromRoute] int invoiceId,
        [FromBody] AddItemsToInvoiceByIdPayload input,
        CancellationToken cancellationToken)
    {
        if (input.Items.Length == 0 || input is null)
            return BadRequest("Add at least one product to the invoice.");

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
        var response = await _invoiceService.PrintInvoiceByIdServiceAsync(invoiceId, cancellationToken);

        return Ok(response);
    }
}
