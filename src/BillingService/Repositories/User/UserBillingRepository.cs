using BillingService.Data;
using BillingService.Models;
using BillingService.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BillingService.Repositories.User;

public class UserBillingRepository : IBillingRepository
{
    private readonly UserBillingDbContext _db;

    public UserBillingRepository(UserBillingDbContext db)
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
