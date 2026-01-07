using BillingService.Data;
using BillingService.Repositories.Interfaces;
using BillingService.Repositories.User;
using BillingService.Repositories.Tenant;

namespace BillingService.Services.Multitenancy;

public interface IBillingContextAccessor
{
    IBillingRepository GetRepository();
}

public sealed class BillingContextAccessor : IBillingContextAccessor
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public BillingContextAccessor(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public IBillingRepository GetRepository()
    {
        var context = _httpContextAccessor.HttpContext
            ?? throw new InvalidOperationException("No active HttpContext");

        if (!context.Items.TryGetValue("DbContext", out var dbContext))
        {
            throw new InvalidOperationException("DbContext not found in HttpContext");
        }

        return dbContext switch
        {
            UserBillingDbContext userDb
                => new UserBillingRepository(userDb),

            TenantBillingDbContext tenantDb
                => new TenantBillingRepository(tenantDb),

            _ => throw new InvalidOperationException(
                $"Unsupported DbContext type: {dbContext.GetType().Name}")
        };
    }
}
