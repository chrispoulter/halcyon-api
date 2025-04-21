namespace Halcyon.Common.Messaging;

public interface IPublisher
{
    Task Publish<T>(IEnumerable<T> message, CancellationToken cancellationToken);
}
