using BillingService.Models;

namespace BillingService.Services.Multitenancy
{
    public interface ITenantResolver
    {
        Tenant? GetTenant(HttpContext context);
        Tenant? GetTenant(string tenantId);
    }

    public class TenantResolver : ITenantResolver
    {
        private readonly ITenantStore _tenantStore;

        public TenantResolver(ITenantStore tenantStore)
        {
            _tenantStore = tenantStore;
        }

        public Tenant? GetTenant(HttpContext context)
        {
            var tenantId = context.Request.Headers["X-Tenant-Id"].FirstOrDefault();
            return tenantId == null ? null : _tenantStore.GetTenant(tenantId);
        }

        public Tenant? GetTenant(string tenantId)
        {
            return _tenantStore.GetTenant(tenantId);
        }
    }
}
