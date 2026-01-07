using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace BillingService.Data.MigrationFactories;

public class TenantBillingDbContextFactory
    : IDesignTimeDbContextFactory<TenantBillingDbContext>
{
    public TenantBillingDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<TenantBillingDbContext>()
            .UseNpgsql(
                "Host=localhost;Port=5432;Database=TenantBillingDb;Username=postgres;Password=yourpassword")
            .Options;

        return new TenantBillingDbContext(options);
    }
}
