using Azure.Storage.Queues;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Halcyon.Web.Services.Events
{
    public abstract class EventBackgroundService<T> : BackgroundService
    {
        private readonly EventSettings _eventSettings;

        private readonly ILogger<EventBackgroundService<T>> _logger;

        public EventBackgroundService(
            IOptions<EventSettings> eventSettings,
            ILogger<EventBackgroundService<T>> logger)
        {
            _eventSettings = eventSettings.Value;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                _logger.LogInformation("Handle Event {event}", typeof(T).Name);

                var delay = true;
                var queueName = typeof(T).Name.ToLower();
                var queue = new QueueClient(_eventSettings.StorageConnectionString, queueName);

                try
                {
                    await queue.CreateIfNotExistsAsync(cancellationToken: cancellationToken);

                    var messages = await queue.ReceiveMessagesAsync(
                        cancellationToken: cancellationToken,
                        maxMessages: _eventSettings.BatchSize);

                    var tasks = messages
                        .Value
                        .Select(async message =>
                        {
                            _logger.LogInformation("Message Received {message}", message.MessageText);

                            try
                            {
                                var data = JsonSerializer.Deserialize<T>(message.MessageText);
                                await ProcessMessage(data);
                            }
                            catch (Exception error)
                            {
                                _logger.LogError(error, "Message Handler Failed");
                            }

                            await queue.DeleteMessageAsync(message.MessageId, message.PopReceipt, cancellationToken);
                        });

                    delay = !tasks.Any();

                    await Task.WhenAll(tasks);
                }
                catch (Exception error)
                {
                    _logger.LogError(error, "Handle Event Failed");
                }

                if (delay)
                {
                    await Task.Delay(_eventSettings.PollingInterval * 1000, cancellationToken);
                }
            }
        }

        public abstract Task ProcessMessage(T data);
    }
}
