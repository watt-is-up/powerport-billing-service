using Microsoft.AspNetCore.Mvc;

using BillingService.Services;
using BillingService.Services.Multitenancy;
using BillingService.Messaging.Publishers;


namespace BillingService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PaymentsController : ControllerBase
{
    private readonly BillingManager _billingManager;
    private readonly ILogger<PaymentsController> _logger;

    public PaymentsController(
        IBillingContextAccessor contextAccessor, 
        IBillingEventPublisher publisher,
        ILogger<PaymentsController> logger)
    {
        var repository = contextAccessor.GetRepository();
        _logger = logger;
        _logger.LogInformation(
            "Using repository type: {RepositoryType}",
            repository.GetType().FullName);
        _billingManager = new BillingManager(repository, publisher);
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

    [HttpPost("finalize")]
    public async Task<IActionResult> FinalizePayment([FromBody] FinalizePaymentRequest request)
    {
        // Check if Auth_user == request.UserId
        var payment = await _billingManager.FinalizePaymentAsync(
            request.SessionId, 
            request.Amount, 
            request.EnergyKwh);
        return Ok(payment);
    }
}

// DTO to receive input from API
public class Payment
{
    public Guid UserId { get; set; }
    public DateTime SessionStarted { get; set; }
    public DateTime SessionEnded { get; set; }
}

public class FinalizePaymentRequest
{
    public Guid UserId { get; set; }
    public Guid SessionId { get; set; }
    public decimal Amount { get; set; }
    public decimal EnergyKwh { get; set; }
}
