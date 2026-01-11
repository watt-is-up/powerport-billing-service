using Confluent.Kafka;
using System.Text.Json;
using BillingService.Messaging.Events;

namespace BillingService.Messaging.Producers;

public class BillingEventProducer
{
    private readonly IProducer<string, string> _producer;

    public BillingEventProducer(IConfiguration config)
    {
        var producerConfig = new ProducerConfig
        {
            BootstrapServers = config["Kafka:BootstrapServers"]
        };

        _producer = new ProducerBuilder<string, string>(producerConfig).Build();
    }

    public async Task PublishAsync<T>(
        string eventType,
        string key,
        T payload)
    {
        var envelope = new EventEnvelope<T>
        {
            EventId = Guid.NewGuid().ToString(),
            EventType = eventType,
            EventVersion = 1,
            OccurredAt = DateTime.UtcNow,
            Producer = "billing-service",
            Key = key,
            Payload = payload
        };

        await _producer.ProduceAsync(
            "billing.events",
            new Message<string, string>
            {
                Key = key,
                Value = JsonSerializer.Serialize(envelope)
            });
    }
}
