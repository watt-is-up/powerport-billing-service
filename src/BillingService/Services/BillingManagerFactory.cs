using BillingService.Repositories.Tenant;
using BillingService.Repositories.User;
using BillingService.Services.Multitenancy;
using BillingService.Messaging.Publishers;

namespace BillingService.Services.Interfaces;
public class BillingManagerFactory : IBillingManagerFactory
{
    private readonly IServiceProvider _sp;
    private readonly ITenantResolver _tenantResolver;

    public BillingManagerFactory(
        IServiceProvider sp, 
        ITenantResolver tenantResolver)
    {
        _sp = sp;
        _tenantResolver = tenantResolver;
    }

    public BillingManager GetManagerForTenant(string tenantId)
    {
        var tenant = _tenantResolver.GetTenant(tenantId);
        
        if (tenant == null)
        {
            throw new InvalidOperationException(
                "Tenant is not registered in Billing service");
        }

        var dbContext = _sp.GetRequiredService<BillingDbContextFactory>()
                            .CreateTenantDbContext(tenant);
        var repo = new TenantBillingRepository(dbContext);
        var publisher = _sp.GetRequiredService<IBillingEventPublisher>();

        return new BillingManager(repo, publisher);

        // DbContext dbContext = tenant != null
        //     ? _sp.GetRequiredService<BillingDbContextFactory>().CreateTenantDbContext(tenant)
        //     : _sp.GetRequiredService<BillingDbContextFactory>().CreateUserDbContext();
        
        // IBillingRepository repo = tenant != null
        //     ? new TenantBillingRepository((TenantBillingDbContext) dbContext)
        //     : new UserBillingRepository((UserBillingDbContext) dbContext);

    }

    public BillingManager GetManagerForUser()
    {
        var dbContext = _sp.GetRequiredService<BillingDbContextFactory>().CreateUserDbContext();
        var publisher = _sp.GetRequiredService<IBillingEventPublisher>();
        return new BillingManager(new UserBillingRepository(dbContext), publisher);
    }
}
