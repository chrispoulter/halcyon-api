using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Halcyon.Api.Data;

public class EntityChangedInterceptor(IPublishEndpoint publishEndpoint) : SaveChangesInterceptor
{
    private readonly List<(object entity, EntityState oldState)> changedEntities = [];

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default
    )
    {
        foreach (var entry in eventData.Context.ChangeTracker.Entries())
        {
            changedEntities.Add((entry.Entity, entry.State));
        }

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    public override async ValueTask<int> SavedChangesAsync(
        SaveChangesCompletedEventData eventData,
        int result,
        CancellationToken cancellationToken = default
    )
    {
        await PublishEntityChangedEvents(cancellationToken);

        return await base.SavedChangesAsync(eventData, result, cancellationToken);
    }

    private async Task PublishEntityChangedEvents(CancellationToken cancellationToken)
    {
        var messages = changedEntities.Select(
            (changedEntity) =>
            {
                var (entity, oldState) = changedEntity;

                return new EntityChangedEvent
                {
                    Id = entity is IEntityWithId entityWithId ? entityWithId.Id : null,
                    ChangeType = oldState,
                    Entity = entity.GetType().Name
                };
            }
        );

        await publishEndpoint.PublishBatch(messages, cancellationToken);
    }
}
