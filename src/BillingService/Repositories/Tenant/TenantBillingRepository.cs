using BillingService.Data;
using BillingService.Models;
using BillingService.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BillingService.Repositories.Tenant;

public class TenantBillingRepository : IBillingRepository
{
    private readonly TenantBillingDbContext _db;

    public TenantBillingRepository(TenantBillingDbContext db)
    {
        _db = db;
    }

    public async Task<List<Payment>> GetPaymentsAsync()
    {
        return await _db.Payments.ToListAsync();
    }

    public async Task<Payment> CreatePaymentAsync(Payment payment)
    {
        _db.Payments.Add(payment);
        await _db.SaveChangesAsync();
        return payment;
    }
}
