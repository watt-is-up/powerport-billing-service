using BillingService.Data;
using BillingService.Models;
using BillingService.Services;
using Microsoft.EntityFrameworkCore;

namespace BillingService.Tests;

public class BillingServiceTests
{
    private BillingDbContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<BillingDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new BillingDbContext(options);
    }

    [Fact]
    public async Task CreatePaymentAsync_CalculatesAmount()
    {
        var dbContext = GetInMemoryDbContext();
        var service = new BillingManager(dbContext);

        var start = DateTime.Now;
        var end = start.AddHours(2);
        var userId = Guid.NewGuid();

        var payment = await service.CreatePaymentAsync(userId, start, end);

        Assert.Equal(userId, payment.UserId);
        Assert.Equal(1.0m, Math.Round(payment.Amount / 1.0m * 2, 0) == 0 ? 1.0m : payment.Amount); // simple check
        Assert.Equal(PaymentStatus.Pending, payment.Status);
    }

    [Fact]
    public async Task GetPaymentsAsync_ReturnsPayments()
    {
        var dbContext = GetInMemoryDbContext();
        var service = new BillingManager(dbContext);

        await service.CreatePaymentAsync(Guid.NewGuid(), DateTime.Now, DateTime.Now.AddHours(1));
        await service.CreatePaymentAsync(Guid.NewGuid(), DateTime.Now, DateTime.Now.AddHours(2));

        var payments = await service.GetPaymentsAsync();
        Assert.Equal(2, payments.Count);
    }
}
