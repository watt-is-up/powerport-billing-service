using BillingService.Data;
using BillingService.Services;
using BillingService.Services.Multitenancy;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// [TODO]: replace with userServiceClient.GetAllProviders()
builder.Services.AddSingleton<ITenantStore>(_ =>
    new InMemoryTenantStore(TenantBootstrap.GetMockTenants()));

// Register the DbContext with dependency injection
builder.Services.AddDbContext<UserBillingDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("SharedBillingDb")));

// Add controllers (for REST API)
builder.Services.AddControllers();
builder.Services.AddSingleton<BillingDbContextFactory>();
builder.Services.AddSingleton<ITenantResolver, TenantResolver>();
builder.Services.AddHttpContextAccessor();

// Register Services
builder.Services.AddScoped<BillingManager>();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseTenantMiddleware();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers(); // Map API endpoints

app.Run();
