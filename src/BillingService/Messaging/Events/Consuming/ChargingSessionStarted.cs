namespace BillingService.Messaging.Events.Consuming;

public class ChargingSessionStarted
{
    public string SessionId { get; set; } = default!;
    public string StationId { get; set; } = default!;
    public string ProviderId { get; set; } = default!;
    public string UserId { get; set; } = default!;
    public DateTime StartedAt { get; set; }
}
