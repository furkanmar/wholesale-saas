using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Wholesale.Application.Common.Interfaces;

namespace Wholesale.Web.Services;

public class CurrentUser : IUser
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUser(IHttpContextAccessor httpContextAccessor)
        => _httpContextAccessor = httpContextAccessor;

    private ClaimsPrincipal? Principal
        => _httpContextAccessor.HttpContext?.User;

    public bool IsAuthenticated
        => Principal?.Identity?.IsAuthenticated == true;

    public Guid? Id
    {
        get
        {
            var sub = Principal?.FindFirstValue(JwtRegisteredClaimNames.Sub);
            return Guid.TryParse(sub, out var id) ? id : null;
        }
    }

    public Guid? TenantId
    {
        get
        {
            var claim = Principal?.FindFirstValue("tenant_id");
            return Guid.TryParse(claim, out var id) ? id : null;
        }
    }

    public string? Role => Principal?.FindFirstValue(ClaimTypes.Role);
}
