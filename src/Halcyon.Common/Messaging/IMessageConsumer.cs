namespace Halcyon.Common.Messaging;

public interface IMessageConsumer<T>
{
    Task Consume(T message, CancellationToken cancellationToken);
}
