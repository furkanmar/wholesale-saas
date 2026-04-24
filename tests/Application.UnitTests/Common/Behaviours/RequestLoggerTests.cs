using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using Wholesale.Application.Common.Behaviours;
using Wholesale.Application.Common.Interfaces;

namespace Wholesale.Application.UnitTests.Common.Behaviours;

// Dummy request — test için gerçek bir command'a ihtiyaç yok
public record PingRequest : IRequest;

public class RequestLoggerTests
{
    private Mock<ILogger<PingRequest>> _logger = null!;
    private Mock<IUser> _user = null!;
    private Mock<IIdentityService> _identityService = null!;

    [SetUp]
    public void Setup()
    {
        _logger = new Mock<ILogger<PingRequest>>();
        _user = new Mock<IUser>();
        _identityService = new Mock<IIdentityService>();
    }

    [Test]
    public async Task ShouldCallGetUsernameAsyncOnceIfAuthenticated()
    {
        var userId = Guid.NewGuid();
        _user.Setup(x => x.Id).Returns(userId);
        _identityService
            .Setup(x => x.GetUsernameAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync("testuser");

        var behaviour = new LoggingBehaviour<PingRequest>(_logger.Object, _user.Object, _identityService.Object);
        await behaviour.Process(new PingRequest(), CancellationToken.None);

        _identityService.Verify(i => i.GetUsernameAsync(userId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task ShouldNotCallGetUsernameAsyncIfUnauthenticated()
    {
        _user.Setup(x => x.Id).Returns((Guid?)null);

        var behaviour = new LoggingBehaviour<PingRequest>(_logger.Object, _user.Object, _identityService.Object);
        await behaviour.Process(new PingRequest(), CancellationToken.None);

        _identityService.Verify(i => i.GetUsernameAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
