using BillingService.Services.Multitenancy;
using BillingService.Infrastructure.Multitenancy;
using BillingService.Messaging.Consumers;
using BillingService.Messaging.Publishers;
using BillingService.Services.Interfaces;
using BillingService.Models;
using Microsoft.EntityFrameworkCore;
using Confluent.Kafka;



var builder = WebApplication.CreateBuilder(args);

//
// -------------------- Configure Services --------------------
//

// 1. Register tenant store (In-memory for now; replace with user service API later)
builder.Services.AddSingleton<ITenantStore>(_ =>
    new InMemoryTenantStore(TenantBootstrap.GetMockTenants()));

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
        var userDb = factory.CreateUserDbContext();
        userDb.Database.Migrate();

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

// Map controllers to endpoints
app.MapControllers();

//
// -------------------- Run App --------------------
app.Run();
