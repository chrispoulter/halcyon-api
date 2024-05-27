namespace Halcyon.Api.Features.Messaging;

public interface IMessagingClient
{
    Task ReceiveMessage(string content);
}
