using BillingService.Models;
using BillingService.Services;
using Microsoft.AspNetCore.Mvc;

namespace BillingService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PaymentsController : ControllerBase
{
    private readonly BillingManager _billingService;

    public PaymentsController(BillingManager billingService)
    {
        _billingService = billingService;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var payments = await _billingService.GetPaymentsAsync();
        return Ok(payments);
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] PaymentDto dto)
    {
        var payment = await _billingService.CreatePaymentAsync(dto.UserId, dto.SessionStarted, dto.SessionEnded);
        return CreatedAtAction(nameof(Get), new { id = payment.Id }, payment);
    }
}

// DTO to receive input from API
public class PaymentDto
{
    public Guid UserId { get; set; }
    public DateTime SessionStarted { get; set; }
    public DateTime SessionEnded { get; set; }
}
