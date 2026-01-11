using Moq;
using FluentAssertions;
using BillingService.Services;
using BillingService.Repositories.Interfaces;
using BillingService.Messaging.Events.Consuming;
using BillingService.Models;
using BillingService.Infrastructure.Multitenancy;

public class BillingManagerTests
{
    private readonly Mock<IBillingRepository> _repo;
    private readonly BillingManager _manager;

    public BillingManagerTests()
    {
        _repo = new Mock<IBillingRepository>();
        _manager = new BillingManager(_repo.Object);
    }

    [Fact]
    public async Task HandleSessionStarted_CreatesPendingPayment()
    {
        var evt = new ChargingSessionStarted
        {
            SessionId = Guid.NewGuid().ToString(),
            UserId = Guid.NewGuid().ToString(),
            ProviderId = "provider-1",
            StartedAt = DateTime.UtcNow
        };

        await _manager.HandleSessionStarted(evt);

        _repo.Verify(r => r.CreatePaymentAsync(
            It.Is<Payment>(p =>
                p.SessionId.ToString() == evt.SessionId &&
                p.Status == PaymentStatus.Pending
            )
        ), Times.Once);
    }

    [Fact]
    public async Task HandleSessionEnded_FinalizesPayment()
    {
        var sessionId = Guid.NewGuid();

        var payment = new Payment
        {
            SessionId = sessionId,
            Status = PaymentStatus.Pending,
            SessionStarted = DateTime.UtcNow.AddHours(-1)
        };

        _repo.Setup(r => r.GetPaymentBySessionAsync(sessionId))
            .ReturnsAsync(payment);

        var evt = new ChargingSessionEnded
        {
            SessionId = sessionId.ToString(),
            ProviderId = "provider-1",
            TotalEnergyKwh = 20m,
            EndedAt = DateTime.UtcNow
        };

        _repo.Setup(r => r.UpdatePaymentBySessionAsync(
            sessionId,
            It.IsAny<Action<Payment>>()))
        .ReturnsAsync((Guid _, Action<Payment> update) =>
        {
            update(payment); // IMPORTANT
            return payment;
        });

        var result = await _manager.HandleSessionEnded(evt);

        result.Status.Should().Be(PaymentStatus.Pending);
        result.EnergyConsumed.Should().Be(20);
    }

}
