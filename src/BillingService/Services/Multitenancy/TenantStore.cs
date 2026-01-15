using BillingService.Models;
using BillingService.Infrastructure.Multitenancy;

public interface ITenantStore
{
    Tenant? GetTenant(string tenantId);
    List<Tenant> GetTenants();
    Tenant? GetTenantByName(string tenantName);
    Tenant GetSharedTenant();
}

public class InMemoryTenantStore : ITenantStore
{
    private readonly Dictionary<string, Tenant> _tenants;
    private readonly string _SharedTenantId;

    public InMemoryTenantStore(IEnumerable<Tenant> tenants)
    {
        _tenants = tenants.ToDictionary(t => t.Id);
        _SharedTenantId = GetTenantByName(TenantsOptions.SharedTenantName)?.Id 
            ?? throw new InvalidOperationException($"Shared tenant '{TenantsOptions.SharedTenantName}' not found");
    }

    public Tenant? GetTenant(string tenantId)
    {
        _tenants.TryGetValue(tenantId, out var tenant);
        return tenant;
    }

    public Tenant? GetTenantByName(string tenantName)
    {
        return _tenants.Values.FirstOrDefault(t => t.Name.Equals(tenantName, StringComparison.OrdinalIgnoreCase));
    }

    public List<Tenant> GetTenants()
    {
        return _tenants.Values.ToList();
    }

    public Tenant GetSharedTenant()
    {
        var tenant = GetTenant(_SharedTenantId);
        if (tenant == null)
        {
            throw new InvalidOperationException("Shared tenant not found");
        }
        return tenant;
    }

}
