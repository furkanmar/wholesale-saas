using Wholesale.Application.Common.Interfaces;

namespace Wholesale.Infrastructure.Identity;

/// <summary>
/// Request başına DI scope'unda yaşar. TenantResolutionMiddleware doldurur.
/// </summary>
public class TenantContext : ITenantContext
{
    public Guid? TenantId { get; private set; }
    public bool IsSuperAdmin { get; private set; }

    public void SetTenant(Guid tenantId) => TenantId = tenantId;
    public void SetSuperAdmin() => IsSuperAdmin = true;
}
