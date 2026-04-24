namespace Wholesale.Application.Common.Interfaces;

/// <summary>
/// Mevcut HTTP isteğini yapan kullanıcı bilgisi.
/// CurrentUser servisi tarafından implemente edilir.
/// </summary>
public interface IUser
{
    Guid? Id { get; }
    Guid? TenantId { get; }
    string? Role { get; }
    bool IsAuthenticated { get; }
}
