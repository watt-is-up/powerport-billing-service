using BillingService.Data;
using BillingService.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Load connection string from appsettings.json
var connectionString = builder.Configuration.GetConnectionString("BillingDb");

// Register the DbContext with dependency injection
builder.Services.AddDbContext<BillingDbContext>(options =>
    options.UseNpgsql(connectionString));

// Add controllers (for REST API)
builder.Services.AddControllers();

// Register Services
builder.Services.AddScoped<BillingManager>();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers(); // Map API endpoints

app.Run();
