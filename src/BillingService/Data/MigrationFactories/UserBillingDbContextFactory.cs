using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace BillingService.Data.MigrationFactories;

public class UserBillingDbContextFactory
    : IDesignTimeDbContextFactory<UserBillingDbContext>
{
    public UserBillingDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<UserBillingDbContext>()
            .UseNpgsql(
                "Host=localhost;Port=5432;Database=UserBillingDb;Username=postgres;Password=yourpassword")
            .Options;

        return new UserBillingDbContext(options);
    }
}
