using Halcyon.Api.Data.Users;
using MassTransit;
using Microsoft.AspNetCore.SignalR;

namespace Halcyon.Api.Features.Notifications;

public class NotifyUserCreatedConsumer(
    IHubContext<NotificationHub, INotificationClient> eventHubContext,
    ILogger<NotifyUserCreatedConsumer> logger
) : IConsumer<Batch<UserCreatedDomainEvent>>
{
    public async Task Consume(ConsumeContext<Batch<UserCreatedDomainEvent>> context)
    {
        foreach (var id in context.Message.Select(m => m.Message.UserId).Distinct())
        {
            var notification = new Notification("UserCreated", new { id });

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
