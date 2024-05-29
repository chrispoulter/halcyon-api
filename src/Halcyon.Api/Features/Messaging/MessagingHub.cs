using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Halcyon.Api.Features.Messaging;

//[Authorize]
public class MessagingHub : Hub<IMessagingClient>
{
    private static readonly List<UserMessage> MessageHistory = [];

    public async Task PostMessage(string content)
    {
        var senderId = Context.ConnectionId;

        var userMessage = new UserMessage(senderId, content, DateTime.UtcNow);

        MessageHistory.Add(userMessage);

        await Clients.Others.ReceiveMessage(senderId, content, userMessage.SentTime);
    }

    public async Task RetrieveMessageHistory() =>
        await Clients.Caller.MessageHistory(MessageHistory);
}
