using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Wholesale.Application.Auth;
using Wholesale.Application.Common.Interfaces;

namespace Wholesale.Application.Auth.RefreshToken;

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, AuthResponse>
{
    private readonly IApplicationDbContext         _db;
    private readonly IJwtTokenService              _jwt;
    private readonly IConfiguration                _config;
    private readonly ILogger<RefreshTokenCommandHandler> _logger;

    public RefreshTokenCommandHandler(
        IApplicationDbContext              db,
        IJwtTokenService                   jwt,
        IConfiguration                     config,
        ILogger<RefreshTokenCommandHandler> logger)
    {
        _db     = db;
        _jwt    = jwt;
        _config = config;
        _logger = logger;
    }

    public async Task<AuthResponse> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Token refresh attempt");

        // Süresi dolmuş token'dan UserId'yi çıkar
        var userId = _jwt.GetUserIdFromExpiredToken(request.AccessToken);

        if (userId is null)
        {
            _logger.LogWarning("Token refresh failed — could not extract UserId from access token");
            throw new UnauthorizedAccessException("Geçersiz token.");
        }

        var user = await _db.Users
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

        if (user is null || !user.IsActive)
        {
            _logger.LogWarning("Token refresh failed — user not found or inactive: {UserId}", userId);
            throw new UnauthorizedAccessException("Geçersiz token.");
        }

        // Refresh token eşleşmesi ve süresi kontrolü
        if (user.RefreshToken != request.RefreshToken ||
            user.RefreshTokenExpiry is null ||
            user.RefreshTokenExpiry <= DateTime.UtcNow)
        {
            _logger.LogWarning(
                "Token refresh failed — invalid or expired refresh token for UserId: {UserId}", userId);
            throw new UnauthorizedAccessException("Refresh token geçersiz veya süresi dolmuş. Lütfen tekrar giriş yapın.");
        }

        // Yeni token çifti üret
        var newAccessToken  = _jwt.GenerateAccessToken(user);
        var newRefreshToken = _jwt.GenerateRefreshToken();

        var refreshDays = int.Parse(_config["Jwt:RefreshTokenDays"] ?? "7");

        user.RefreshToken       = newRefreshToken;
        user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(refreshDays);

        await _db.SaveChangesAsync(cancellationToken);

        var expiresMinutes = int.Parse(_config["Jwt:AccessTokenMinutes"] ?? "60");
        var expiresAt = DateTime.UtcNow.AddMinutes(expiresMinutes);

        _logger.LogInformation(
            "Token refreshed successfully for UserId: {UserId}, Role: {Role}",
            user.Id, user.Role);

        return new AuthResponse(newAccessToken, newRefreshToken, expiresAt);
    }
}
