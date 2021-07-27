using Azure.Storage.Queues;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Halcyon.Web.Services.Events
{
    public abstract class EventBackgroundService<T> : BackgroundService
    {
        private readonly EventSettings _eventSettings;

        private readonly ILogger<EventHandler> _logger;

        public EventBackgroundService(
            IOptions<EventSettings> eventSettings,
            ILogger<EventHandler> logger)
        {
            _eventSettings = eventSettings.Value;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var queueName = typeof(T).Name.ToLower();

                    _logger.LogInformation("Event Background Service Executing: {date}", DateTime.UtcNow);

                    var queue = new QueueClient(_eventSettings.StorageConnectionString, queueName);
                    
                    await queue.CreateIfNotExistsAsync(cancellationToken: stoppingToken);

                    var messages = await queue.ReceiveMessagesAsync(stoppingToken);

                    foreach (var message in messages.Value)
                    {
                        _logger.LogInformation(
                            "Event Background Service Message Received: {queue} {message}",
                            queueName, 
                            message.MessageText);

                        var data = JsonSerializer.Deserialize<T>(message.MessageText);

                        await HandleEventAsync(data);

                        await queue.DeleteMessageAsync(message.MessageId, message.PopReceipt, stoppingToken);
                    }

                    await Task.Delay(10 * 1000, stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    return;
                }
                catch (Exception error)
                {
                    _logger.LogError(error, "Event Background Service Failed");
                }
            }
        }

        public abstract Task HandleEventAsync(T data);
    }
}
