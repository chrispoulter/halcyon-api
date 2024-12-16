using System.Security.Claims;
using Microsoft.AspNetCore.SignalR;

namespace Halcyon.Api.Features.Notifications;

public class NotificationHub(ILogger<NotificationHub> logger) : Hub<INotificationClient>
{
    public override async Task OnConnectedAsync()
    {
        logger.LogInformation(
            "Connection to {Hub} started, UserId: {UserId}",
            nameof(NotificationHub),
            Context.User?.Identity?.Name
        );

        var user = Context.User;

        if (user.Identity.IsAuthenticated)
        {
            var roles = user.FindAll(ClaimTypes.Role);

            var groups = roles
                .Select(role => GetGroupForRole(role.Value))
                .Append(GetGroupForUser(user.Identity.Name))
                .ToArray();

            logger.LogInformation("Adding connection to groups {Groups}", groups);

            var addToGroup = groups.Select(group =>
                Groups.AddToGroupAsync(Context.ConnectionId, group)
            );

            await Task.WhenAll(addToGroup);
        }

        await base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception exception)
    {
        logger.LogInformation(
            "Connection to {Hub} ended, Exception: {Exception}",
            nameof(NotificationHub),
            exception
        );

        return base.OnDisconnectedAsync(exception);
    }

    public static string GetGroupForUser(object id) => $"USER_{id}";

    public static string GetGroupForRole(string role) => $"ROLE_{role}";
}
