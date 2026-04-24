using System.Reflection;
using Wholesale.Application.Common.Exceptions;
using Wholesale.Application.Common.Interfaces;
using Wholesale.Application.Common.Security;

namespace Wholesale.Application.Common.Behaviours;

public class AuthorizationBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly IUser _user;

    public AuthorizationBehaviour(IUser user) => _user = user;

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var authorizeAttributes = request.GetType().GetCustomAttributes<AuthorizeAttribute>();

        if (authorizeAttributes.Any())
        {
            if (!_user.IsAuthenticated || _user.Id == null)
                throw new UnauthorizedAccessException();

            // Role-based check
            var withRoles = authorizeAttributes.Where(a => !string.IsNullOrWhiteSpace(a.Roles));
            if (withRoles.Any())
            {
                var authorized = withRoles
                    .SelectMany(a => a.Roles.Split(',', StringSplitOptions.TrimEntries))
                    .Any(role => _user.Role == role);

                if (!authorized)
                    throw new ForbiddenAccessException();
            }
        }

        return await next();
    }
}
