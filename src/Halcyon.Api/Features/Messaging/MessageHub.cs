using System.Security.Claims;
using Microsoft.AspNetCore.SignalR;

namespace Halcyon.Api.Features.Messaging;

public class MessageHub : Hub<IMessageClient>
{
    public override async Task OnConnectedAsync()
    {
        var user = Context.User;

        if (user.Identity.IsAuthenticated)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, GetGroupForUser(user.Identity.Name));

            var roles = user.FindAll(ClaimTypes.Role);

            foreach (var role in roles)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, GetGroupForRole(role.Value));
            }
        }

        await base.OnConnectedAsync();
    }

    public static string GetGroupForUser(object id) => $"MESSAGE_USER_{id}";

    public static string GetGroupForRole(string role) => $"MESSAGE_ROLE_{role}";
}
