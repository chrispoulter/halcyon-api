using Azure.Storage.Queues;
using Halcyon.Web.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace Halcyon.Web.Services.Events
{
    public class EventService : IEventService
    {
        private readonly EventSettings _eventSettings;

        private readonly ILogger<EventService> _logger;

        public EventService(
            IOptions<EventSettings> eventSettings,
            ILogger<EventService> logger)
        {
            _eventSettings = eventSettings.Value;
            _logger = logger;
        }

        public async Task PublishEventAsync<T>(T data)
        {
            var queueName = data.GetType().Name.ToLower();
            var queue = new QueueClient(_eventSettings.StorageConnectionString, queueName);
            var message = JsonSerializer.Serialize(data);

            try
            {
                await queue.CreateIfNotExistsAsync();
                await queue.SendMessageAsync(message);
            }
            catch (Exception error)
            {
                _logger.LogError(error, "Publish Event Failed");
            }
        }
    }
}
