using BillingService.Data;
using BillingService.Models;
using Microsoft.EntityFrameworkCore;

public class BillingDbContextFactory
{  
    private readonly IConfiguration _configuration;

    public BillingDbContextFactory(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public TenantBillingDbContext CreateTenantDbContext(Tenant tenant)
    {
        var options = new DbContextOptionsBuilder<TenantBillingDbContext>()
            .UseSqlServer(tenant.ConnectionString) // or UseNpgsql
            .Options;

        return new TenantBillingDbContext(options);
    }

    public UserBillingDbContext CreateUserDbContext()
    {
        var settings = _configuration.GetSection("ConnectionStrings");
        var options = new DbContextOptionsBuilder<UserBillingDbContext>()
            .UseSqlServer(settings["SharedBillingDb"]) // or UseNpgsql
            .Options;

        return new UserBillingDbContext(options);
    }
}
