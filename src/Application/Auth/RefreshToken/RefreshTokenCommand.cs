using Wholesale.Application.Auth;

namespace Wholesale.Application.Auth.RefreshToken;

/// <summary>
/// Süresi dolmuş access token + geçerli refresh token ile yeni token çifti alma komutu.
/// </summary>
public record RefreshTokenCommand(string AccessToken, string RefreshToken) : IRequest<AuthResponse>;
