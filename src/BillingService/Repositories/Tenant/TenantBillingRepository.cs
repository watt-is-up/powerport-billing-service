using BillingService.Data;
using BillingService.Models;
using BillingService.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace BillingService.Repositories.Tenant;

public class TenantBillingRepository : IBillingRepository
{
    private readonly TenantBillingDbContext _db;

    public TenantBillingRepository(TenantBillingDbContext db)
    {
        // create DbContext per tenant once
        _db = db;
    }


    public async Task<List<Payment>> GetPaymentsAsync()
    {
        return await _db.Payments.ToListAsync();
    }

    public async Task<Payment?> GetPaymentBySessionAsync(Guid sessionId)
    {
        return await _db.Payments.SingleOrDefaultAsync(p => p.SessionId == sessionId);
    }

    public async Task<Payment> CreatePaymentAsync(Payment payment)
    {
        _db.Payments.Add(payment);
        await _db.SaveChangesAsync();
        return payment;
    }

    public async Task<Payment> UpdatePaymentAsync(Payment payment)
    {
        _db.Payments.Update(payment);
        await _db.SaveChangesAsync();
        return payment;
    }

    public async Task<Payment> UpdatePaymentBySessionAsync(
        Guid sessionId,
        Action<Payment> update)
    {
        var payment = await _db.Payments
            .SingleAsync(p => p.SessionId == sessionId);

        update(payment);

        await _db.SaveChangesAsync();
        return payment;
    }

}
