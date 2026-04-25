namespace Wholesale.Application.Auth;

/// <summary>
/// Login ve token yenileme işlemlerinin ortak dönüş tipi.
/// </summary>
public record AuthResponse(
    string   AccessToken,
    string   RefreshToken,
    DateTime ExpiresAt);
