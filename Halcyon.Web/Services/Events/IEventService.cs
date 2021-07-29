using System;
using System.Threading;
using System.Threading.Tasks;

namespace Halcyon.Web.Services.Events
{
    public interface IEventService
    {
        Task PublishEventAsync<T>(T message);

        Task HandleEventAsync<T>(Func<T, Task> messageHandler, CancellationToken cancellationToken);
    }
}
