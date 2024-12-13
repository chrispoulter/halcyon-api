using Halcyon.Api.Data.Users;
using MassTransit;
using Microsoft.AspNetCore.SignalR;

namespace Halcyon.Api.Features.Notifications;

public class NotifyUserUpdatedConsumer(
    IHubContext<NotificationHub, INotificationClient> eventHubContext,
    ILogger<NotifyUserUpdatedConsumer> logger
) : IConsumer<Batch<UserUpdatedDomainEvent>>
{
    public async Task Consume(ConsumeContext<Batch<UserUpdatedDomainEvent>> context)
    {
        foreach (var id in context.Message.Select(m => m.Message.UserId).Distinct())
        {
            var notification = new Notification("UserUpdated", new { id });

            var groups = new[]
            {
                NotificationHub.GetGroupForRole(Roles.SystemAdministrator),
                NotificationHub.GetGroupForRole(Roles.UserAdministrator),
                NotificationHub.GetGroupForUser(id),
            };

            logger.LogInformation(
                "Sending notification for {Notification} to {Groups}, UserId: {UserId}",
                notification.Type,
                groups,
                id
            );

            await eventHubContext
                .Clients.Groups(groups)
                .ReceiveNotification(notification, context.CancellationToken);
        }
    }
}
