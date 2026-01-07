using BillingService.Models;
using Microsoft.EntityFrameworkCore;

namespace BillingService.Data;

// DbContext is the bridge between C# code and the database.
// It keeps track of all entities and can perform queries and updates.
public class TenantBillingDbContext : DbContext
{
    public TenantBillingDbContext(DbContextOptions<TenantBillingDbContext> options)
        : base(options)
    {
    }

    public DbSet<Payment> Payments => Set<Payment>();

    // Optional: customize table names or relationships
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.Property(p => p.Amount)
                .HasPrecision(18, 2);

            entity.Property(p => p.EnergyConsumed)
                .HasPrecision(18, 6);

            entity.Property(p => p.Rate)
                .HasPrecision(18, 6);
        });

    }
}
