using Halcyon.Api.Core.Database;

namespace Halcyon.Api.Features.Messaging;

public interface IMessageClient
{
    Task ReceiveMessage(EntityChangedEvent message, CancellationToken cancellationToken);
}
