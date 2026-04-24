using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Wholesale.Application.Common.Interfaces;

namespace Wholesale.Application.FunctionalTests.Infrastructure;

public class WebApiFactory(string connectionString) : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseSetting("ConnectionStrings:DefaultConnection", connectionString);

        builder.ConfigureTestServices(services =>
        {
            services
                .RemoveAll<IUser>()
                .AddTransient(_ =>
                {
                    var mock = new Mock<IUser>();
                    mock.SetupGet(x => x.Id).Returns(TestApp.GetUserId());
                    mock.SetupGet(x => x.Role).Returns(TestApp.GetRole());
                    mock.SetupGet(x => x.IsAuthenticated).Returns(TestApp.GetUserId().HasValue);
                    return mock.Object;
                });
        });
    }
}
