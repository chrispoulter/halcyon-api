using Halcyon.Api.Data;

namespace Halcyon.Api.Features.Messaging;

public interface IMessageClient
{
    Task ReceiveMessage(EntityChangedEvent message, CancellationToken cancellationToken);
}
