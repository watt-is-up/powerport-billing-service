using System;

namespace BillingService.Models
{
    // This class represents a Payment in the system.
    // EF Core will map it to a "Payments" table in PostgreSQL.
    public class Payment
    {

        /// <summary>
        /// Primary key, auto-incremented
        /// </summary>
        public int Id { get; set; } // Primary key, auto-incremented
        /// <summary>
        /// Who is paying
        /// </summary>
        public Guid UserId { get; set; } // Who is paying
        /// <summary>
        /// Who is paying
        /// </summary>
        public Guid ProviderId { get; set; } // Who is paying
        /// <summary>
        /// Charging session reference
        /// </summary>
        public Guid SessionId { get; set; } // Charging session reference
        /// <summary>
        /// Calculated amount
        /// </summary>
        public decimal Amount { get; set; } // Calculated amount
        /// <summary>
        /// Session start
        /// </summary>
        public DateTime SessionStarted { get; set; } // Session start
        /// <summary>
        /// Session end
        /// </summary>
        public DateTime SessionEnded { get; set; } // Session end
        /// <summary>
        /// kWh consumed
        /// </summary>
        public decimal EnergyConsumed { get; set; } // kWh consumed
        /// <summary>
        /// rice per kWh
        /// </summary>
        public decimal Rate { get; set; } // Price per kWh
        /// <summary>
        /// Payment status: Pending, Paid, Failed
        /// </summary>
        public PaymentStatus Status { get; set; } // Payment status: Pending, Paid, Failed
        /// <summary>
        /// Creation time
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // Creation time
        /// <summary>
        /// Last update
        /// </summary>
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow; // Last update
    }
}
