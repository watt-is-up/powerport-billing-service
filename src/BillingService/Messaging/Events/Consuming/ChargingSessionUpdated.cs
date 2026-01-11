namespace BillingService.Messaging.Events.Consuming;

public class ChargingSessionUpdated
{
    public string SessionId { get; set; } = default!;
    public string ProviderId { get; set; } = default!;
    public float EnergyConsumedKwh { get; set; }
    public DateTime UpdatedAt { get; set; }
}
