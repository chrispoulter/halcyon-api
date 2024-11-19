using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace Halcyon.Api.Core.Database;

public class MigrationHostedService<TDbContext>(
    IServiceProvider serviceProvider,
    ILogger<MigrationHostedService<TDbContext>> logger
) : IHostedService
    where TDbContext : DbContext
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Migrating database for {DbContext}",
            TypeCache<TDbContext>.ShortName
        );

        using var scope = serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<TDbContext>();

        try
        {
            await dbContext.Database.MigrateAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "An error occurred while migrating database for {DbContext}",
                TypeCache<TDbContext>.ShortName
            );
        }

        logger.LogInformation("Seeding database for {DbContext}", TypeCache<TDbContext>.ShortName);

        var dbSeeder = scope.ServiceProvider.GetRequiredService<IDbSeeder<TDbContext>>();

        try
        {
            await dbSeeder.SeedAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "An error occurred while seeding database for {DbContext}",
                TypeCache<TDbContext>.ShortName
            );
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
