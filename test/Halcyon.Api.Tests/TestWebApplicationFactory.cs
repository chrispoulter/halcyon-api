using Halcyon.Api.Core.Email;
using Halcyon.Api.Data;
using MassTransit;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;
using Testcontainers.MsSql;

namespace Halcyon.Api.Tests;

public class TestWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly MsSqlContainer dbContainer = new MsSqlBuilder()
        .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
        .Build();

    public readonly Mock<IEmailSender> MockEmailSender = new();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll<DbContextOptions<HalcyonDbContext>>();

            services.AddDbContext<HalcyonDbContext>(options =>
                options.UseSqlServer(dbContainer.GetConnectionString())
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

    public Task InitializeAsync() => dbContainer.StartAsync();

    public new Task DisposeAsync() => dbContainer.DisposeAsync().AsTask();
}
