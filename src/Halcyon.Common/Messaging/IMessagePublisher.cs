namespace Halcyon.Common.Messaging;

public interface IMessagePublisher
{
    Task Publish<T>(IEnumerable<T> message, CancellationToken cancellationToken);
}
