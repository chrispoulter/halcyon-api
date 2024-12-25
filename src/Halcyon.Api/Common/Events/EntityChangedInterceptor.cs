using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Halcyon.Api.Common.Events;

public class EntityChangedInterceptor(IPublishEndpoint publishEndpoint) : SaveChangesInterceptor
{
    private readonly List<(IEntity entity, EntityState oldState)> changedEntities = [];

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default
    )
    {
        var entries = eventData.Context.ChangeTracker.Entries<IEntity>();
        var changes = entries.Select(entry => (entry.Entity, entry.State));
        changedEntities.AddRange(changes);

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
                return new EntityChangedEvent(entity.GetType().Name, oldState, entity.Id);
            }
        );

        await publishEndpoint.PublishBatch(messages, cancellationToken);
    }
}
