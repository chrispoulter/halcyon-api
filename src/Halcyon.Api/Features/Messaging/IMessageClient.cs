
namespace Halcyon.Api.Features.Messaging;

public interface IMessageClient
{
    void ReceiveMessage(Message message, CancellationToken cancellationToken);
}
