using BillingService.Data;
using BillingService.Models;
using Microsoft.EntityFrameworkCore;


namespace BillingService.Services;

public class BillingManager
{
    private readonly BillingDbContext _dbContext;

    public BillingManager(BillingDbContext dbContext)
    {
        _dbContext = dbContext;
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

        _dbContext.Payments.Add(payment);
        await _dbContext.SaveChangesAsync();

        return payment;
    }

    // Get all payments
    public async Task<List<Payment>> GetPaymentsAsync()
    {
        return await _dbContext.Payments.ToListAsync();
    }
}
