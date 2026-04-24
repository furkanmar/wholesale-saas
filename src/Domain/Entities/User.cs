using Wholesale.Domain.Common;

namespace Wholesale.Domain.Entities;

public class User : BaseAuditableEntity
{
    public Guid TenantId { get; set; }

    public string Username { get; set; } = string.Empty;

    public string PasswordHash { get; set; } = string.Empty;

    /// <summary>SuperAdmin | Manager | Staff | Customer</summary>
    public string Role { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;

    /// <summary>Sadece Customer rolündeki kullanıcılar için dolu olur.</summary>
    public Guid? LinkedCustomerId { get; set; }

    public string? RefreshToken { get; set; }

    public DateTime? RefreshTokenExpiry { get; set; }

    // Navigation
    public Tenant? Tenant { get; set; }
}
