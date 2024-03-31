using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace Halcyon.Api.Services.Migrations;

public class MigrationHostedService<TDbContext> : IHostedService
    where TDbContext : DbContext
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<MigrationHostedService<TDbContext>> _logger;

    public MigrationHostedService(
        IServiceProvider serviceProvider,
        ILogger<MigrationHostedService<TDbContext>> logger
    )
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Migrating database for {DbContext}",
            TypeCache<TDbContext>.ShortName
        );

        using var scope = _serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<TDbContext>();

        if (!dbContext.Database.IsRelational())
        {
            _logger.LogWarning(
                "Cannot apply migrations to non relational database for {DbContext}",
                TypeCache<TDbContext>.ShortName
            );

            return;
        }

        try
        {
            await dbContext.Database.MigrateAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "An error occurred while migrating database for {DbContext}",
                TypeCache<TDbContext>.ShortName
            );
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
