using Microsoft.AspNetCore.Mvc;

using BillingService.Services;
using BillingService.Services.Multitenancy;


namespace BillingService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PaymentsController : ControllerBase
{
    private readonly BillingManager _billingManager;

    public PaymentsController(IBillingContextAccessor contextAccessor)
    {
        var repository = contextAccessor.GetRepository();
        _billingManager = new BillingManager(repository);
    }

    [HttpGet]
    public async Task<IActionResult> GetPayments()
    {
        var payments = await _billingManager.GetPaymentsAsync();
        return Ok(payments);
    }

    [HttpPost]
    public async Task<IActionResult> AddPayment([FromBody] Payment payment)
    {
        var createdPayment = await _billingManager.CreatePaymentAsync(payment.UserId, payment.SessionStarted, payment.SessionEnded);
        return CreatedAtAction(nameof(GetPayments), new { id = createdPayment.Id }, createdPayment);
    }
}

// DTO to receive input from API
public class Payment
{
    public Guid UserId { get; set; }
    public DateTime SessionStarted { get; set; }
    public DateTime SessionEnded { get; set; }
}
