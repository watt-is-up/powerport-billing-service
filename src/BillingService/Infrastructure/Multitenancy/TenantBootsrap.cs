using BillingService.Models;
public static class TenantBootstrap
{
    public static List<Tenant> GetMockTenants() => new()
    {
        new Tenant
        {
            Id = "provider-a",
            Name = "Provider A",
            ConnectionString = "Server=localhost;Database=Billing_ProviderA;User Id=sa;Password=Your_password123;"
        },
        new Tenant
        {
            Id = "provider-b",
            Name = "Provider B",
            ConnectionString = "Server=localhost;Database=Billing_ProviderB;User Id=sa;Password=Your_password123;"
        }
    };
}
