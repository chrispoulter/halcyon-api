using Halcyon.Api.Data;
using Halcyon.Api.Services.Email;
using MassTransit;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;
using Testcontainers.PostgreSql;

namespace Halcyon.Api.Tests;

public class TestWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer dbContainer = new PostgreSqlBuilder()
        .WithImage("postgres:17.0")
        .Build();

    public readonly Mock<IEmailService> MockEmailService = new();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll<DbContextOptions<HalcyonDbContext>>();

            services.AddDbContext<HalcyonDbContext>(options =>
                options.UseNpgsql(dbContainer.GetConnectionString()).UseSnakeCaseNamingConvention()
            );

            services.AddMassTransitTestHarness(options => options.UsingInMemory());

            services
                .AddAuthentication(TestAuthenticationHandler.AuthenticationScheme)
                .AddScheme<TestAuthenticationOptions, TestAuthenticationHandler>(
                    TestAuthenticationHandler.AuthenticationScheme,
                    options => { }
                );

            services.AddScoped((_) => MockEmailService.Object);
        });
    }

    public Task InitializeAsync() => dbContainer.StartAsync();

    public new Task DisposeAsync() => dbContainer.DisposeAsync().AsTask();
}
