using BillingService.Models;

namespace BillingService.Infrastructure.Multitenancy;
public static class TenantBootstrap
{
    public static List<Tenant> GetMockTenants() => new()
    {
        new Tenant
        {
            Id = "b7e6c9d1-5e7a-4c9d-8a31-1c7f2d9b8a01",
            Name = "Provider A",
            ConnectionString = "Host=postgres-db;Port=5432;Database=provider_adb;Username=providera;Password=secretpassword"
        },
        new Tenant
        {
            Id = "e8b1c7d6-5f4a-4e9b-8c2d-1a7f6b9e5c02",
            Name = "Provider B",
            ConnectionString = "Host=postgres-db;Port=5432;Database=provider_bdb;Username=providerb;Password=secretpassword"
        }
    };
}
