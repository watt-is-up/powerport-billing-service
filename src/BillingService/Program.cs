using BillingService.Services.Multitenancy;
using BillingService.Infrastructure.Multitenancy;
using BillingService.Messaging.Consumers;
using BillingService.Messaging.Publishers;
using BillingService.Services.Interfaces;
using BillingService.Models;
using Microsoft.EntityFrameworkCore;
using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using HealthChecks.NpgSql;
using System.Text.Json;  



var builder = WebApplication.CreateBuilder(args);

//
// -------------------- Configure Services --------------------
//

// 1. Register tenant store (In-memory for now; replace with user service API later)
builder.Services.AddSingleton<ITenantStore>(_ =>
    new InMemoryTenantStore(TenantBootstrap.LoadFromConfiguration(builder.Configuration)));

// Tenant DbContexts will be created dynamically via BillingDbContextFactory
builder.Services.AddSingleton<BillingDbContextFactory>();

// 2. Multitenancy helpers
builder.Services.AddSingleton<ITenantResolver, TenantResolver>(); // resolves tenant per request
builder.Services.AddHttpContextAccessor();                        // required to access HttpContext in services
builder.Services.AddScoped<IBillingContextAccessor, BillingContextAccessor>(); 
// IBillingContextAccessor resolves the correct repository (User or Tenant) per request
builder.Services.AddScoped<IBillingManagerFactory, BillingManagerFactory>();

// 3. Add Messaging consumers and producers and publishers
builder.Services.AddSingleton<IProducer<string, string>>(sp =>
{
    var config = new ProducerConfig
    {
        BootstrapServers = builder.Configuration["Kafka:BootstrapServers"]
    };
    return new ProducerBuilder<string, string>(config).Build();
});
// Consumers
builder.Services.AddHostedService<ChargingSessionConsumer>();
// Publishers
builder.Services.AddScoped<IBillingEventPublisher, KafkaBillingEventPublisher>();

// 4. Add controllers (REST API)
builder.Services.AddControllers();

// 5. Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add health checks
builder.Services.AddHealthChecks()
    .AddCheck(
        "self",
        () => HealthCheckResult.Healthy(),
        tags: new[] { "live "})
    .AddNpgSql(
        builder.Configuration.GetConnectionString("BillingDb"),
        tags: new[] { "ready" }
    );

//
// -------------------- Build App --------------------
var app = builder.Build();

// -------------------- Apply Migrations --------------------
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var factory = services.GetRequiredService<BillingDbContextFactory>();
    var tenantStore = services.GetRequiredService<ITenantStore>();

    try
    {
        // var userDb = factory.CreateUserDbContext();
        // userDb.Database.Migrate();

        foreach(Tenant tenant in tenantStore.GetTenants())
        {
            var tenantDb = factory.CreateTenantDbContext(tenant);
            tenantDb.Database.Migrate();
        }
        
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while migrating the databases.");
        throw; // Fail fast if migrations fail
    }
}

//
// -------------------- Configure Middleware --------------------
//

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

// Multitenancy middleware
// Determines the current tenant (or shared DB) and attaches the correct DbContext to HttpContext
app.UseTenantMiddleware();

//
// -------------------- Configure Request Pipeline --------------------
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// configure health endpoints
app.MapHealthChecks("/health/live", new HealthCheckOptions
{
    Predicate = r => r.Tags.Contains("live"),
    ResponseWriter = WriteHealthResponse
})
.WithTags("Health")
.AllowAnonymous();

app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = r => r.Tags.Contains("ready"),
    ResponseWriter = WriteHealthResponse
})
.WithTags("Health")
.AllowAnonymous();

// Map controllers to endpoints
app.MapControllers();

//
// -------------------- Run App --------------------
app.Run();


static Task WriteHealthResponse(HttpContext context, HealthReport report)
{
    context.Response.ContentType = "application/json";

    var result = JsonSerializer.Serialize(new
    {
        status = report.Status.ToString(),
        checks = report.Entries.Select(e => new
        {
            name = e.Key,
            status = e.Value.Status.ToString(),
            description = e.Value.Description
        })
    });

    return context.Response.WriteAsync(result);
}