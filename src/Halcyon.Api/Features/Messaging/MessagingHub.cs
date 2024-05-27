using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Halcyon.Api.Features.Messaging;

//[Authorize]
public class MessagingHub : Hub<IMessagingClient>
{
    public async Task SendMessage(string message) =>
        await Clients.All.ReceiveMessage(message);

    public async Task SendMessageToCaller(string message) =>
        await Clients.Caller.ReceiveMessage(message);

    public async Task SendMessageToUserAdministrators(string message) =>
        await Clients.Group("USER_ADMINISTRATOR").ReceiveMessage(message);
}
