using Wholesale.Application.Auth;

namespace Wholesale.Application.Auth.Login;

/// <summary>
/// Kullanıcı adı + şifre ile giriş yapma komutu.
/// </summary>
public record LoginCommand(string Username, string Password) : IRequest<AuthResponse>;
