namespace Halcyon.Common.Messaging;

public interface IConsumer<T>
{
    Task Consume(T message, CancellationToken cancellationToken);
}
