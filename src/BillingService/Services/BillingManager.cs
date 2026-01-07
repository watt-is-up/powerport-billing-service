using BillingService.Data;
using BillingService.Models;
using BillingService.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;


namespace BillingService.Services;

public class BillingManager
{
    private readonly IBillingRepository _repository;

    public BillingManager(IBillingRepository repository)
    {
        _repository = repository;
    }

    // Calculates amount and saves a payment
    public async Task<Payment> CreatePaymentAsync(Guid userId, DateTime start, DateTime end)
    {
        var duration = (end - start).TotalHours;
        var ratePerHour = 0.5m; // Example rate
        var amount = (decimal)duration * ratePerHour;

        var payment = new Payment
        {
            UserId = userId,
            SessionStarted = start,
            SessionEnded = end,
            Amount = amount,
            Status = PaymentStatus.Pending
        };

        return await _repository.CreatePaymentAsync(payment);
    }

    // Get all payments
    public async Task<List<Payment>> GetPaymentsAsync()
    {
        return await _repository.GetPaymentsAsync();
    }
}
