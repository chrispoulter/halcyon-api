using Azure.Storage.Queues;
using Microsoft.Extensions.Options;
using System.Text.Json;
using System.Threading.Tasks;

namespace Halcyon.Web.Services.Events
{
    public class EventService : IEventService
    {
        private readonly EventSettings _eventSettings;

        public EventService(IOptions<EventSettings> eventSettings)
        {
            _eventSettings = eventSettings.Value;
        }

        public async Task PublishEventAsync<T>(T data)
        {
            var queueName = data.GetType().Name.ToLower();
            var queue = new QueueClient(_eventSettings.StorageConnectionString, queueName);
            await queue.CreateIfNotExistsAsync();

            var message = JsonSerializer.Serialize(data);
            await queue.SendMessageAsync(message);
        }
    }
}
