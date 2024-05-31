namespace Halcyon.Api.Features.Messaging;

public interface IMessageClient
{
    void ReceiveMessage(MessageEvent message, CancellationToken cancellationToken);
}
