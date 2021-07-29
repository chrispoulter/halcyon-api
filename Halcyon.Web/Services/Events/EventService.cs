using Azure.Storage.Queues;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Text.Json;
using System.Threading;
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

        public async Task HandleEventAsync<T>(Func<T, Task> messageHandler, CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                _logger.LogInformation("Handle Event {event}", typeof(T).Name);

                var queueName = typeof(T).Name.ToLower();
                var queue = new QueueClient(_eventSettings.StorageConnectionString, queueName);

                try
                {
                    await queue.CreateIfNotExistsAsync(cancellationToken: cancellationToken);

                    var messages = await queue.ReceiveMessagesAsync(
                        cancellationToken: cancellationToken,
                        maxMessages: _eventSettings.BatchSize);

                    await Task.WhenAll(messages
                        .Value
                        .Select(async message =>
                        {
                            _logger.LogInformation("Message Received {message}", message.MessageText);

                            try
                            {
                                var data = JsonSerializer.Deserialize<T>(message.MessageText);
                                await  messageHandler(data);
                            }
                            catch (Exception error)
                            {
                                _logger.LogError(error, "Message Handler Failed");
                            }
                            finally
                            {
                                await queue.DeleteMessageAsync(message.MessageId, message.PopReceipt, cancellationToken);
                            }
                        }));
                }
                catch (Exception error)
                {
                    _logger.LogError(error, "Handle Event Failed");
                }

                await Task.Delay(_eventSettings.PollingInterval * 1000, cancellationToken);
            }
        }
    }
}
