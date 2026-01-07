using BillingService.Data;
using Microsoft.EntityFrameworkCore;

namespace BillingService.Services.Multitenancy
{
    public class TenantMiddleware
{
    private readonly RequestDelegate _next;

    public TenantMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(
            HttpContext context,
            ITenantResolver tenantResolver,
            BillingDbContextFactory factory)
        {
            var tenant = tenantResolver.GetTenant(context);

            DbContext dbContext;

            if (tenant == null)
            {
                // Normal user → shared DB
                dbContext = factory.CreateUserDbContext();
            }
            else
            {
                // Provider → tenant DB
                if (string.IsNullOrEmpty(tenant.ConnectionString))
                {
                    context.Response.StatusCode = 400;
                    await context.Response.WriteAsync("Tenant connection string missing");
                    return;
                }

                dbContext = factory.CreateTenantDbContext(tenant);
            }

            context.Items["DbContext"] = dbContext;

            await _next(context);

            if (tenant != null)
                await dbContext.DisposeAsync();
        }

    }


    public static class TenantMiddlewareExtensions
    {
        public static IApplicationBuilder UseTenantMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<TenantMiddleware>();
        }
    }
}
