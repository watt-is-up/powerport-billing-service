using BillingService.Models;

namespace BillingService.Infrastructure.Multitenancy;
public static class TenantBootstrap
{
    public static List<Tenant> GetMockTenants() => new()
    {
        new Tenant
        {
            Id = "5a8d3c1f-9e42-4b7d-8f06-2c91e7a4d6b5",
            Name = "ina",
            ConnectionString = "Host=postgres-db;Port=5432;Database=billing_ina_db;Username=billing_ina_user;Password=secretpassword"
        },
        new Tenant
        {
            Id = "91c4b7a2-6d3e-4f89-a5c1-0e2d8b6f7a43",
            Name = "petrol",
            ConnectionString = "Host=postgres-db;Port=5432;Database=billing_petrol_db;Username=billing_petrol_user;Password=secretpassword"
        },
        new Tenant
        {
            Id = "2e9f6a7b-4c1d-4e3a-9b8f-6d1a7c5e4b04",
            Name = "makpetrol",
            ConnectionString = "Host=postgres-db;Port=5432;Database=billing_makpetrol_db;Username=billing_makpetrol_user;Password=secretpassword"
        }
    };

    public static List<Tenant> LoadFromConfiguration(IConfiguration configuration)
    {
        var tenantsSection = configuration.GetSection(TenantsOptions.SectionName);
        var tenants = new List<Tenant>();

        foreach (var child in tenantsSection.GetChildren())
        {
            var name = child.Key;
            var id = child["Id"];
            var connectionString = child["ConnectionString"];

            if (string.IsNullOrWhiteSpace(id) || string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException($"Tenant '{name}' is missing Id or ConnectionString");
            }

            tenants.Add(new Tenant
            {
                Id = id,
                Name = name,
                ConnectionString = connectionString
            });
        }

        if (tenants.Count == 0)
        {
            throw new InvalidOperationException("No tenants configured in 'Tenants' section");
        }

        return tenants;
    }
}

public sealed class TenantsOptions
{
    public const string SectionName = "Tenants";
    public const string SharedTenantName = "shared";

    public Dictionary<string, TenantConfig> Tenants { get; set; } = new();
}

public sealed class TenantConfig
{
    public string Id { get; set; } = default!;
    public string ConnectionString { get; set; } = default!;
}