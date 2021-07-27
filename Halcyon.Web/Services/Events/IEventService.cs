using System.Threading.Tasks;

namespace Halcyon.Web.Services.Events
{
    public interface IEventService
    {
        Task PublishEventAsync<T>(T message);
    }
}
