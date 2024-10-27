using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace Halcyon.Api.Features.Messaging;

public class MessageHub : Hub<IMessageClient>
{
    public override async Task OnConnectedAsync()
    {
        var user = Context.User;

        if (user.Identity.IsAuthenticated)
        {
            var roles = user.FindAll(ClaimTypes.Role);

            var groups = roles
                .Select(role => GetGroupForRole(role.Value))
                .Append(GetGroupForUser(user.Identity.Name));

            var addToGroup = groups.Select(group =>
                Groups.AddToGroupAsync(Context.ConnectionId, group)
            );

            await Task.WhenAll(addToGroup);
        }

        await base.OnConnectedAsync();
    }

    public static string GetGroupForUser(object id) => $"USER_{id}";

    public static string GetGroupForRole(string role) => $"ROLE_{role}";
}