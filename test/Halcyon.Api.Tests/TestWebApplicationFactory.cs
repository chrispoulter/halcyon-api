using Halcyon.Api.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.PostgreSql;

namespace Halcyon.Api.Tests;

public class TestWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer dbContainer = new PostgreSqlBuilder().Build();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var dbContextDescriptor = services.SingleOrDefault(d =>
                d.ServiceType == typeof(DbContextOptions<HalcyonDbContext>)
            );

            if (dbContextDescriptor is not null)
            {
                services.Remove(dbContextDescriptor);
            }

            services
                .AddDbContext<HalcyonDbContext>(
                    (provider, options) =>
                    {
                        options
                            .UseNpgsql(dbContainer.GetConnectionString())
                            .UseSnakeCaseNamingConvention();
                    }
                )
                .EnsureDatabaseCreated();

            services
                .AddAuthentication(TestAuthenticationHandler.AuthenticationScheme)
                .AddScheme<TestAuthenticationOptions, TestAuthenticationHandler>(
                    TestAuthenticationHandler.AuthenticationScheme,
                    options => { }
                );
        });

        builder.UseEnvironment("Development");
    }

    public Task InitializeAsync()
    {
        return dbContainer.StartAsync();
    }

    public new Task DisposeAsync()
    {
        return dbContainer.DisposeAsync().AsTask();
    }
}
