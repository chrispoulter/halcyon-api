namespace Halcyon.Api.Common.Messaging;

public interface IMessageConsumer<TMessage>
{
    Task Consume(TMessage message, CancellationToken cancellationToken);
}
