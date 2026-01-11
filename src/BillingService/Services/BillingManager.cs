using BillingService.Models;
using BillingService.Repositories.Interfaces;
using BillingService.Messaging.Events.Consuming;
using BillingService.Messaging.Events.Emitting;
using BillingService.Messaging.Publishers;
using BillingService.Data.Migrations.Tenant;


namespace BillingService.Services;

public class BillingManager
{
    private readonly IBillingRepository _repository;
    private readonly IBillingEventPublisher _eventPublisher;


    public BillingManager(IBillingRepository repository, IBillingEventPublisher eventPublisher)
    {
        _repository = repository;
        _eventPublisher = eventPublisher;
    }

    private async Task<decimal> CalculatePaymentAmount(Payment payment)
    {
        var duration = (payment.SessionEnded - payment.SessionStarted).TotalHours;
        var ratePerHour = 0.5m; // Example rate
        var amount = (decimal)duration * ratePerHour;
        return amount;
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

    public async Task<Payment?> HandleSessionStarted(ChargingSessionStarted startedEvent)
    {
        if (!Guid.TryParse(startedEvent.UserId, out var UserId))
            throw new InvalidOperationException("Invalid UserId");
        if (!Guid.TryParse(startedEvent.SessionId, out var SessionId))
            throw new InvalidOperationException("Invalid SessionId");
        if (!Guid.TryParse(startedEvent.ProviderId, out var ProviderId))
            throw new InvalidOperationException("Invalid ProviderId");


        try
        {
            var inDbPayment = await _repository.GetPaymentBySessionAsync(SessionId);
            if (inDbPayment != null)
            {
                return null;
            }

            var payment = new Payment
            {
                UserId = UserId,
                SessionId = SessionId,
                ProviderId = ProviderId,
                SessionStarted = startedEvent.StartedAt,
                Status = PaymentStatus.Pending
            };

            return await _repository.CreatePaymentAsync(payment);
        }
        catch (Exception ex)
        {
            throw; // important: rethrow so Kafka can retry if needed
        }
    }

    public async Task<Payment?> HandleSessionUpdate(ChargingSessionUpdated updateEvent)
    {
        if (!Guid.TryParse(updateEvent.SessionId, out var SessionId))
            throw new InvalidOperationException("Invalid SessionId");

        try
        {
            var payment = await _repository.GetPaymentBySessionAsync(SessionId);
            if (payment == null)
            {
                return null;
            }

            return await _repository.UpdatePaymentBySessionAsync(
                SessionId,
                p =>
                {
                    p.UpdatedAt = updateEvent.UpdatedAt;
                    p.EnergyConsumed = (decimal) updateEvent.EnergyConsumedKwh;
                });
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<Payment?> HandleSessionEnded(ChargingSessionEnded endedEvent)
    {
        if (!Guid.TryParse(endedEvent.SessionId, out var SessionId))
            throw new InvalidOperationException("Invalid SessionId");

        try
        {
            var payment = await _repository.GetPaymentBySessionAsync(SessionId);
            if (payment == null)
            {   
                return null;
            }
            
            var amount = await CalculatePaymentAmount(payment);
            return await _repository.UpdatePaymentBySessionAsync(
                SessionId,
                p =>
                {
                    p.SessionEnded = endedEvent.EndedAt;
                    p.UpdatedAt = endedEvent.EndedAt;
                    p.EnergyConsumed = endedEvent.TotalEnergyKwh;
                    p.Amount = amount;
                });
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<Payment> FinalizePaymentAsync(Guid sessionId, decimal amount, decimal energyKwh)
    {

        var payment = await _repository.GetPaymentBySessionAsync(sessionId);

        if (payment == null)
            throw new InvalidOperationException("This session doesn't exist");
        if (payment.Status == PaymentStatus.Paid)
            throw new InvalidOperationException("This session has been finalized");
        if (amount != payment.Amount)
            throw new InvalidOperationException("Payment amount is not correct");

        try
        {
            // 1. Update payment in DB
            payment.Status = PaymentStatus.Paid;
            await _repository.UpdatePaymentAsync(payment);
        
            // 2. Publish event
            await _eventPublisher.PublishSessionFinalizedAsync(new ChargingSessionFinalized
            {
                SessionId = payment.Id.ToString(),
                UserId = payment.UserId.ToString(),
                ProviderId = payment.ProviderId.ToString(),
                Amount = amount,
                SessionStarted = payment.SessionStarted,
                SessionEnded = payment.SessionEnded,
                TotalEnergyKwh = energyKwh
            });

        } catch (Exception ex)
        {
            throw new InvalidOperationException("Payment didn't finalize correctly");
        }
        
        return payment;
    }
}
