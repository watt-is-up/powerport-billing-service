using BillingService.Models;

namespace BillingService.Repositories.Interfaces;

public interface IBillingRepository
{
    Task<List<Payment>> GetPaymentsAsync();
    Task<Payment> CreatePaymentAsync(Payment payment);
}
