using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Wholesale.Application.Common.Interfaces;

namespace Wholesale.Application.Common.Behaviours;

public class PerformanceBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly Stopwatch _timer = new();
    private readonly ILogger<TRequest> _logger;
    private readonly IUser _user;
    private readonly IIdentityService _identityService;

    public PerformanceBehaviour(ILogger<TRequest> logger, IUser user, IIdentityService identityService)
    {
        _logger = logger;
        _user = user;
        _identityService = identityService;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        _timer.Restart();
        var response = await next();
        _timer.Stop();

        if (_timer.ElapsedMilliseconds > 500)
        {
            var requestName = typeof(TRequest).Name;
            var userId = _user.Id;
            string? userName = null;

            if (userId.HasValue)
                userName = await _identityService.GetUsernameAsync(userId.Value, cancellationToken);

            _logger.LogWarning("Long Running Request: {Name} ({Elapsed}ms) {@UserId} {@UserName} {@Request}",
                requestName, _timer.ElapsedMilliseconds, userId, userName, request);
        }

        return response;
    }
}
