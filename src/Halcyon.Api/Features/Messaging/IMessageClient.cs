namespace Halcyon.Api.Features.Messaging;

public interface IMessageClient
{
    Task ReceiveMessage(MessageEvent message, CancellationToken cancellationToken);
}
