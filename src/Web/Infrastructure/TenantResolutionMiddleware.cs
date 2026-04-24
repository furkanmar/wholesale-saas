using System.Security.Claims;
using Wholesale.Domain.Constants;
using Wholesale.Infrastructure.Identity;

namespace Wholesale.Web.Infrastructure;

/// <summary>
/// JWT claim'lerinden tenant_id ve role okuyarak TenantContext'i doldurur.
/// UseAuthentication'dan sonra pipeline'a eklenmeli.
/// </summary>
public class TenantResolutionMiddleware
{
    private readonly RequestDelegate _next;

    public TenantResolutionMiddleware(RequestDelegate next) => _next = next;

    public async Task InvokeAsync(HttpContext context, TenantContext tenantContext)
    {
        if (context.User.Identity?.IsAuthenticated == true)
        {
            var role = context.User.FindFirstValue(ClaimTypes.Role);

            if (role == Roles.SuperAdmin)
            {
                tenantContext.SetSuperAdmin();
            }
            else
            {
                var tenantClaim = context.User.FindFirstValue("tenant_id");
                if (Guid.TryParse(tenantClaim, out var tenantId))
                    tenantContext.SetTenant(tenantId);
            }
        }

        await _next(context);
    }
}
