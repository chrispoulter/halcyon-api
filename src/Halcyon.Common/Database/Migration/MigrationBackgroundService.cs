using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Halcyon.Common.Database.Migration;

public class MigrationBackgroundService<TDbContext>(
    IServiceProvider serviceProvider,
    ILogger<MigrationBackgroundService<TDbContext>> logger
) : BackgroundService
    where TDbContext : DbContext
{
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Migrating database for {DbContext}", typeof(TDbContext).Name);

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
                typeof(TDbContext).Name
            );

            return;
        }

        var dbSeeders = scope.ServiceProvider.GetServices<IDbSeeder<TDbContext>>();

        if (!dbSeeders.Any())
        {
            return;
        }

        foreach (var seeder in dbSeeders)
        {
            logger.LogInformation(
                "Seeding database for {DbContext} with {DbSeeder}",
                typeof(TDbContext).Name,
                seeder.GetType().Name
            );

            try
            {
                await seeder.SeedAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                logger.LogError(
                    ex,
                    "An error occurred while seeding database for {DbContext} with {DbSeeder}",
                    typeof(TDbContext).Name,
                    seeder.GetType().Name
                );
            }
        }
    }
}
