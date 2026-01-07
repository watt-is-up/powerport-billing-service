
namespace BillingService.Models
{
    public class Tenant
    {
        public required string Id { get; set; }
        public required string Name { get; set; }
        public required string ConnectionString { get; set; }
    }

}
