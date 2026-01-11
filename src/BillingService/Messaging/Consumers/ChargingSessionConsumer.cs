using Confluent.Kafka;
using System.Text.Json;

using BillingService.Messaging.Events.Consuming;
using BillingService.Messaging.Events;
using BillingService.Services.Interfaces;

namespace BillingService.Messaging.Consumers;

public class ChargingSessionConsumer : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IConfiguration _configuration;
    private readonly ILogger<ChargingSessionConsumer> _logger;

    public ChargingSessionConsumer(
        IServiceScopeFactory scopeFactory,
        IConfiguration configuration,
        ILogger<ChargingSessionConsumer> logger)
    {
        _scopeFactory = scopeFactory;
        _configuration = configuration;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {

        await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);

        var config = new ConsumerConfig
        {
            BootstrapServers = _configuration["Kafka:BootstrapServers"],
            GroupId = "billing-service.charging-session.events.v1",
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = false
        };

        using var consumer = new ConsumerBuilder<string, string>(config).Build();
        consumer.Subscribe("charging-session.events");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var result = consumer.Consume(stoppingToken);

                if (string.IsNullOrWhiteSpace(result.Message.Value))
                {
                    // skip empty messages (console producer sends them)
                    continue;
                }

                using var scope = _scopeFactory.CreateScope();
                var factory = scope.ServiceProvider.GetRequiredService<IBillingManagerFactory>();

                var envelope = JsonSerializer.Deserialize<EventEnvelope<JsonElement>>(result.Message.Value)!;

                try
                {
                    await HandleEvent(envelope, factory);
                    consumer.Commit(result);
                }
                catch (Exception ex)
                {
                    _logger.LogError(
                        ex,
                        "Failed to process Kafka event. Topic: {Topic}, Key: {Key}, EventType: {EventType}, EventId: {EventId}",
                        result.Topic,
                        result.Message.Key,
                        envelope.EventType,
                        envelope.EventId
                    );

                    // Do NOT commit – Kafka will retry
                }
            }
            catch (ConsumeException ex) when (ex.Error.Code == ErrorCode.UnknownTopicOrPart)
            {
                await Task.Delay(2000, stoppingToken);
            }
        }
    }

    private static async Task HandleEvent(
        EventEnvelope<JsonElement> envelope,
        IBillingManagerFactory factory)
    {

        switch (envelope.EventType)
        {
            case "ChargingSessionStarted":
                var started = envelope.Payload.Deserialize<ChargingSessionStarted>()!;
                await factory.GetManagerForTenant(started.ProviderId)
                            .HandleSessionStarted(started);

                await factory.GetManagerForUser()
                            .HandleSessionStarted(started);
                break;

            case "ChargingSessionUpdated":
                var update = envelope.Payload.Deserialize<ChargingSessionUpdated>()!;
                await factory.GetManagerForTenant(update.ProviderId)
                            .HandleSessionUpdate(update);

                await factory.GetManagerForUser()
                            .HandleSessionUpdate(update);
                break;

            case "ChargingSessionEnded":
                var ended = envelope.Payload.Deserialize<ChargingSessionEnded>()!;
                await factory.GetManagerForTenant(ended.ProviderId)
                            .HandleSessionEnded(ended);

                await factory.GetManagerForUser()
                            .HandleSessionEnded(ended);
                break;
        }
    }
}
