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
                _logger.LogInformation("Event Background Service Executing");

                var queueName = typeof(T).Name.ToLower();
                var queue = new QueueClient(_eventSettings.StorageConnectionString, queueName);

                try
                {
                    await queue.CreateIfNotExistsAsync(cancellationToken: stoppingToken);

                    var messages = await queue.ReceiveMessagesAsync(
                        cancellationToken: stoppingToken,
                        maxMessages: _eventSettings.BatchSize);

                    var tasks = messages.Value
                        .Select(async message =>
                        {
                            _logger.LogInformation(
                                    "Event Background Service Message Received: {event} {message}",
                                    typeof(T).Name,
                                    message.MessageText);

                            try
                            {
                                var data = JsonSerializer.Deserialize<T>(message.MessageText);
                                await HandleEventAsync(data);
                            }
                            catch (Exception error)
                            {
                                _logger.LogError(error, "Event Background Service Message Failed");
                            }
                            finally
                            {
                                await queue.DeleteMessageAsync(message.MessageId, message.PopReceipt, stoppingToken);
                            }
                        });

                    await Task.WhenAll(tasks);
                }
                catch (Exception error)
                {
                    _logger.LogError(error, "Event Background Service Failed");
                }
                finally
                { 
                    await Task.Delay(_eventSettings.PollingInterval * 1000, stoppingToken);
                }
            }
        }

        public abstract Task HandleEventAsync(T data);
    }
}
