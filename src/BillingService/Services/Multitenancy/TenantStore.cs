using BillingService.Models;

public interface ITenantStore
{
    Tenant? GetTenant(string tenantId);
}

public class InMemoryTenantStore : ITenantStore
{
    private readonly Dictionary<string, Tenant> _tenants;

    public InMemoryTenantStore(IEnumerable<Tenant> tenants)
    {
        _tenants = tenants.ToDictionary(t => t.Id);
    }

    public Tenant? GetTenant(string tenantId)
    {
        _tenants.TryGetValue(tenantId, out var tenant);
        return tenant;
    }
}
