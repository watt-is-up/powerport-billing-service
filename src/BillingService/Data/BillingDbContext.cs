using BillingService.Models;
using Microsoft.EntityFrameworkCore;

namespace BillingService.Data
{
    // DbContext is the bridge between C# code and the database.
    // It keeps track of all entities and can perform queries and updates.
    public class BillingDbContext : DbContext
    {
        public BillingDbContext(DbContextOptions<BillingDbContext> options)
            : base(options)
        {
        }

        // DbSet represents a table in the database
        public DbSet<Payment> Payments { get; set; }

        // Optional: customize table names or relationships
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Payment>()
                .Property(p => p.Amount)
                .HasColumnType("decimal(18,2)"); // Ensure proper decimal precision
        }
    }
}
