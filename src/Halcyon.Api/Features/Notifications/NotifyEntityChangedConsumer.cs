using Halcyon.Api.Common.Events;
using Halcyon.Api.Data.Users;
using MassTransit;
using Microsoft.AspNetCore.SignalR;

namespace Halcyon.Api.Features.Notifications;

public class NotifyEntityChangedConsumer(
    IHubContext<NotificationHub, INotificationClient> eventHubContext,
    ILogger<NotifyEntityChangedConsumer> logger
) : IConsumer<Batch<EntityChangedEvent>>
{
    public async Task Consume(ConsumeContext<Batch<EntityChangedEvent>> context)
    {
        foreach (var item in context.Message)
        {
            var message = item.Message;

            switch (message.Entity)
            {
                case nameof(User):
                    var groups = new[]
                    {
                        NotificationHub.GetGroupForRole(Roles.SystemAdministrator),
                        NotificationHub.GetGroupForRole(Roles.UserAdministrator),
                        NotificationHub.GetGroupForUser(message.Id),
                    };

                    logger.LogInformation(
                        "Sending notification for {Event} to {Groups}, Type: {EntityType}, State: {EntityState}, Id: {EntityId}",
                        nameof(EntityChangedEvent),
                        groups,
                        message.Entity,
                        message.State,
                        message.Id
                    );

                    var notification = new Notification(nameof(EntityChangedEvent), message);

                    await eventHubContext
                        .Clients.Groups(groups)
                        .ReceiveNotification(notification, context.CancellationToken);

                    break;
            }
        }
    }
}
