using Microsoft.AspNetCore.Mvc;
using Wholesale.Application.Auth;
using Wholesale.Application.Auth.Login;
using Wholesale.Application.Auth.RefreshToken;
using Wholesale.Web.Infrastructure;

namespace Wholesale.Web.Endpoints.Auth;

/// <summary>
/// Kimlik doğrulama endpoint'leri — tüm route'lar anonymous erişime açıktır.
/// </summary>
public class AuthEndpoints : IEndpointGroup
{
    public static string? RoutePrefix => "/api/v1/auth";

    public static void Map(RouteGroupBuilder group)
    {
        group.MapPost(Login).AllowAnonymous()
            .Produces<AuthResponse>(200)
            .ProducesProblem(401)
            .ProducesProblem(422)
            .WithSummary("Giriş yap")
            .WithDescription("Kullanıcı adı ve şifre ile giriş yaparak access + refresh token alır.");

        group.MapPost(Refresh, "refresh").AllowAnonymous()
            .Produces<AuthResponse>(200)
            .ProducesProblem(401)
            .ProducesProblem(422)
            .WithSummary("Token yenile")
            .WithDescription("Süresi dolmuş access token ve geçerli refresh token ile yeni token çifti alır.");
    }

    private static async Task<IResult> Login(
        [FromBody] LoginCommand command,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(command, cancellationToken);
        return Results.Ok(result);
    }

    private static async Task<IResult> Refresh(
        [FromBody] RefreshTokenCommand command,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(command, cancellationToken);
        return Results.Ok(result);
    }
}
