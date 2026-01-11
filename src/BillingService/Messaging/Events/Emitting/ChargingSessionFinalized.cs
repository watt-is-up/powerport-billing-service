namespace BillingService.Messaging.Events.Emitting;

public class ChargingSessionFinalized
{
    public int Id { get; set; }
    public string UserId { get; set; } = default!;
    public string ProviderId { get; set; } = default!;
    public string SessionId { get; set; } = default!;
    public decimal Amount { get; set; } 
    public DateTime SessionStarted { get; set; } 
    public DateTime SessionEnded { get; set; } 
    public decimal TotalEnergyKwh { get; set; } 
    public decimal Rate { get; set; } 
}