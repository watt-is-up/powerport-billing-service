using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace BillingService.Data.MigrationFactories;

public class UserBillingDbContextFactory
    : IDesignTimeDbContextFactory<UserBillingDbContext>
{
    public UserBillingDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<UserBillingDbContext>()
            .UseSqlServer(
                "Server=(localdb)\\mssqllocaldb;Database=UserBillingDb;Trusted_Connection=True;")
            .Options;

        return new UserBillingDbContext(options);
    }
}
