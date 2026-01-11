using Confluent.Kafka;
using System.Text.Json;

using BillingService.Messaging.Events;
using BillingService.Messaging.Events.Emitting;

namespace BillingService.Messaging.Publishers;
public interface IBillingEventPublisher
{
    Task PublishSessionFinalizedAsync(ChargingSessionFinalized evt);
}

// Implementation
public class KafkaBillingEventPublisher : IBillingEventPublisher
{
    private readonly IProducer<string, string> _producer;
    
    public KafkaBillingEventPublisher(IProducer<string, string> producer)
    {
        _producer = producer;
    }
    
    public async Task PublishSessionFinalizedAsync(ChargingSessionFinalized evt)
    {
        var envelope = new EventEnvelope<ChargingSessionFinalized>
        {
            EventId = Guid.NewGuid().ToString(),
            EventType = "ChargingSessionFinalized",
            EventVersion = 1,
            OccurredAt = DateTime.UtcNow,
            Producer = "billing-service",
            Key = evt.SessionId,
            Payload = evt
        };
        
        await _producer.ProduceAsync("billing.events", new Message<string, string>
        {
            Key = evt.SessionId,
            Value = JsonSerializer.Serialize(envelope)
        });
    }
}