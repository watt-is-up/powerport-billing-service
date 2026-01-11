using BillingService.Models;

namespace BillingService.Repositories.Interfaces;

public interface IBillingRepository
{
    Task<List<Payment>> GetPaymentsAsync();
    Task<Payment?> GetPaymentBySessionAsync(Guid sessionId);
    Task<Payment> CreatePaymentAsync(Payment payment);
    Task<Payment> UpdatePaymentAsync(Payment payment);
    Task<Payment> UpdatePaymentBySessionAsync(
        Guid sessionId, Action<Payment> update);
}
