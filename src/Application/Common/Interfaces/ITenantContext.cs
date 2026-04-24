namespace Wholesale.Application.Common.Interfaces;

public interface ITenantContext
{
    Guid? TenantId { get; }
    bool IsSuperAdmin { get; }
}
