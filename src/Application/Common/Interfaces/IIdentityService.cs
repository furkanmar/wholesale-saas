namespace Wholesale.Application.Common.Interfaces;

/// <summary>
/// Kullanıcı kimlik doğrulama işlemleri için uygulama katmanı sözleşmesi.
/// </summary>
public interface IIdentityService
{
    Task<string?> GetUsernameAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<bool> IsInRoleAsync(Guid userId, string role, CancellationToken cancellationToken = default);
}
