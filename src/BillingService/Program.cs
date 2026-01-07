using BillingService.Data;
using BillingService.Services.Multitenancy;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

//
// -------------------- Configure Services --------------------
//

// 1. Register tenant store (In-memory for now; replace with user service API later)
builder.Services.AddSingleton<ITenantStore>(_ =>
    new InMemoryTenantStore(TenantBootstrap.GetMockTenants()));

// 2. Register DbContexts

// // Shared database for normal users
// builder.Services.AddDbContext<UserBillingDbContext>(options =>
//     options.UseSqlServer(
//         builder.Configuration.GetConnectionString("SharedBillingDb")));

// Tenant DbContexts will be created dynamically via BillingDbContextFactory
builder.Services.AddSingleton<BillingDbContextFactory>();

// 3. Multitenancy helpers
builder.Services.AddSingleton<ITenantResolver, TenantResolver>(); // resolves tenant per request
builder.Services.AddHttpContextAccessor();                        // required to access HttpContext in services
builder.Services.AddScoped<IBillingContextAccessor, BillingContextAccessor>(); 
// IBillingContextAccessor resolves the correct repository (User or Tenant) per request

// 4. Add controllers (REST API)
builder.Services.AddControllers();

// 5. Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//
// -------------------- Build App --------------------
var app = builder.Build();

//
// -------------------- Configure Middleware --------------------
//

app.UseHttpsRedirection();

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
