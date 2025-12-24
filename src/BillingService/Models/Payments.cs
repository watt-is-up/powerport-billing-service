using System;

namespace BillingService.Models
{
    // This class represents a Payment in the system.
    // EF Core will map it to a "Payments" table in PostgreSQL.
    public class Payment
    {
        public int Id { get; set; } // Primary key, auto-incremented
        public Guid UserId { get; set; } // Who is paying
        public Guid SessionId { get; set; } // Charging session reference
        public decimal Amount { get; set; } // Calculated amount
        public DateTime SessionStarted { get; set; } // Session start
        public DateTime SessionEnded { get; set; } // Session end
        public decimal EnergyConsumed { get; set; } // kWh consumed
        public decimal Rate { get; set; } // Price per kWh
        public PaymentStatus Status { get; set; } // Payment status: Pending, Paid, Failed
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // Creation time
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow; // Last update
    }
}
