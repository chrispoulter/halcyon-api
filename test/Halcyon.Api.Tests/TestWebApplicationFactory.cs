using Castle.Core.Smtp;
using Halcyon.Api.Data;
using MassTransit;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;

namespace Halcyon.Api.Tests;

public class TestWebApplicationFactory : WebApplicationFactory<Program>
{
    public readonly Mock<IEmailSender> MockEmailSender = new();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll(typeof(DbContextOptions<HalcyonDbContext>));

            services.AddDbContext<HalcyonDbContext>(options =>
                options.UseInMemoryDatabase("HalcyonTestDatabase")
            );

            services.AddMassTransitTestHarness(cfg =>
                cfg.SetTestTimeouts(testInactivityTimeout: TimeSpan.FromSeconds(3))
            );

            services
                .AddAuthentication(TestAuthenticationHandler.AuthenticationScheme)
                .AddScheme<TestAuthenticationOptions, TestAuthenticationHandler>(
                    TestAuthenticationHandler.AuthenticationScheme,
                    options => { }
                );

            services.AddScoped((_) => MockEmailSender.Object);
        });
    }
}
