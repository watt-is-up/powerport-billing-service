namespace BillingService.Services.Interfaces;
public interface IBillingManagerFactory
{
    BillingManager GetManagerForTenant(string tenantId);
    // BillingManager GetManagerForUser();
}
