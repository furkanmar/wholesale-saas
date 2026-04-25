using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Wholesale.Application.Auth;
using Wholesale.Application.Common.Interfaces;

namespace Wholesale.Application.Auth.Login;

public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResponse>
{
    private readonly IApplicationDbContext       _db;
    private readonly IJwtTokenService            _jwt;
    private readonly IPasswordHasher             _hasher;
    private readonly IConfiguration              _config;
    private readonly ILogger<LoginCommandHandler> _logger;

    public LoginCommandHandler(
        IApplicationDbContext        db,
        IJwtTokenService             jwt,
        IPasswordHasher              hasher,
        IConfiguration               config,
        ILogger<LoginCommandHandler> logger)
    {
        _db     = db;
        _jwt    = jwt;
        _hasher = hasher;
        _config = config;
        _logger = logger;
    }

    public async Task<AuthResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Login attempt for username: {Username}", request.Username);

        // Tenant filtresi login sırasında geçersiz — IgnoreQueryFilters zorunlu
        var user = await _db.Users
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(u => u.Username == request.Username, cancellationToken);

        if (user is null || !_hasher.Verify(request.Password, user.PasswordHash))
        {
            _logger.LogWarning("Login failed — invalid credentials for username: {Username}", request.Username);
            throw new UnauthorizedAccessException("Kullanıcı adı veya şifre hatalı.");
        }

        if (!user.IsActive)
        {
            _logger.LogWarning("Login failed — inactive user: {UserId}", user.Id);
            throw new UnauthorizedAccessException("Hesabınız aktif değil. Lütfen yöneticinizle iletişime geçin.");
        }

        // Token üret
        var accessToken  = _jwt.GenerateAccessToken(user);
        var refreshToken = _jwt.GenerateRefreshToken();

        var refreshDays = int.Parse(_config["Jwt:RefreshTokenDays"] ?? "7");

        user.RefreshToken       = refreshToken;
        user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(refreshDays);

        await _db.SaveChangesAsync(cancellationToken);

        var expiresMinutes = int.Parse(_config["Jwt:AccessTokenMinutes"] ?? "60");
        var expiresAt = DateTime.UtcNow.AddMinutes(expiresMinutes);

        _logger.LogInformation(
            "Login successful for UserId: {UserId}, Role: {Role}, TenantId: {TenantId}",
            user.Id, user.Role, user.TenantId);

        return new AuthResponse(accessToken, refreshToken, expiresAt);
    }
}
