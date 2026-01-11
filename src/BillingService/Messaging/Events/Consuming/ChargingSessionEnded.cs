namespace BillingService.Messaging.Events.Consuming;

public class ChargingSessionEnded
{
    public string SessionId { get; set; } = default!;
    public string ProviderId { get; set; } = default!;
    public decimal TotalEnergyKwh { get; set; }
    public DateTime EndedAt { get; set; }
}
