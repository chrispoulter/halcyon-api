namespace Halcyon.Api.Features.Messaging;

public interface IMessagingClient
{
    Task ReceiveMessage(string senderId, string content, DateTime sentTime);

    Task MessageHistory(List<UserMessage> messageHistory);
}
