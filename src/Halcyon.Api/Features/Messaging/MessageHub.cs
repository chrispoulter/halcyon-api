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
            await Groups.AddToGroupAsync(Context.ConnectionId, $"USER_{user.Identity.Name}");

            var roles = user.FindAll(ClaimTypes.Role);

            foreach (var role in roles)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, $"ROLE_{role.Value}");
            }
        }

        await base.OnConnectedAsync();
    }
}
