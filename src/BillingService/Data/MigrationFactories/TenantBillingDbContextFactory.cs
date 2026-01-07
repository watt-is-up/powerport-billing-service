using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace BillingService.Data.MigrationFactories;

public class TenantBillingDbContextFactory
    : IDesignTimeDbContextFactory<TenantBillingDbContext>
{
    public TenantBillingDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<TenantBillingDbContext>()
            .UseSqlServer(
                "Server=(localdb)\\mssqllocaldb;Database=TenantBillingDb;Trusted_Connection=True;")
            .Options;

        return new TenantBillingDbContext(options);
    }
}
