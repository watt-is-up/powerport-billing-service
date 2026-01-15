using Confluent.Kafka;
using System.Text.Json;

using BillingService.Messaging.Events.Consuming;
using BillingService.Messaging.Events;
using BillingService.Services.Interfaces;
using BillingService.Services.Multitenancy;

namespace BillingService.Messaging.Consumers;

public class ChargingSessionConsumer : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IConfiguration _configuration;
    private readonly ILogger<ChargingSessionConsumer> _logger;
    private readonly ITenantResolver _tenantResolver;

    public ChargingSessionConsumer(
        IServiceScopeFactory scopeFactory,
        IConfiguration configuration,
        ILogger<ChargingSessionConsumer> logger,
        ITenantResolver tenantResolver)
    {
        _scopeFactory = scopeFactory;
        _configuration = configuration;
        _logger = logger;
        _tenantResolver = tenantResolver;
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
                    _logger.LogWarning("Received empty message from Kafka, skipping.");
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

    private async Task HandleEvent(
        EventEnvelope<JsonElement> envelope,
        IBillingManagerFactory factory)
    {
        _logger.LogInformation("Processing {EventType} for key {Key}", envelope.EventType, envelope.Key);
        var sharedTenant = _tenantResolver.GetSharedTenant();
        
        switch (envelope.EventType)
        {
            case "ChargingSessionStarted":
                var started = envelope.Payload.Deserialize<ChargingSessionStarted>()!;
                await factory.GetManagerForTenant(started.ProviderId)
                            .HandleSessionStarted(started);

                if (sharedTenant == null) throw new Exception("Shared tenant not found");
                await factory.GetManagerForTenant(sharedTenant.Id)
                            .HandleSessionStarted(started);
                // await factory.GetManagerForUser()
                //             .HandleSessionStarted(started);
                break;

            case "ChargingSessionUpdated":
                var update = envelope.Payload.Deserialize<ChargingSessionUpdated>()!;
                await factory.GetManagerForTenant(update.ProviderId)
                            .HandleSessionUpdate(update);

                if (sharedTenant == null) throw new Exception("Shared tenant not found");
                await factory.GetManagerForTenant(sharedTenant.Id)
                            .HandleSessionUpdate(update);

                // await factory.GetManagerForUser()
                //             .HandleSessionUpdate(update);
                break;

            case "ChargingSessionEnded":
                var ended = envelope.Payload.Deserialize<ChargingSessionEnded>()!;
                await factory.GetManagerForTenant(ended.ProviderId)
                            .HandleSessionEnded(ended);
                
                if (sharedTenant == null) throw new Exception("Shared tenant not found");
                await factory.GetManagerForTenant(sharedTenant.Id)
                            .HandleSessionEnded(ended);
                // await factory.GetManagerForUser()
                //             .HandleSessionEnded(ended);
                break;

            default:
                _logger.LogWarning("Unknown event type: {EventType}", envelope.EventType);
                return;
        }

        _logger.LogInformation("Successfully processed {EventType} for key {Key}", envelope.EventType, envelope.Key);
    }
}
