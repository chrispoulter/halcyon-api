using System.Text;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Halcyon.Common.Messaging;

public partial class MessageBackgroundService<T>(
    IServiceProvider serviceProvider,
    IConnectionFactory connectionFactory,
    ILogger<MessageBackgroundService<T>> logger
) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        //try
        //{
        using var connection = await connectionFactory.CreateConnectionAsync(cancellationToken);

        using var channel = await connection.CreateChannelAsync(
            cancellationToken: cancellationToken
        );

        var queue = typeof(T).FullName;

        await channel.QueueDeclareAsync(
            queue,
            durable: true,
            exclusive: false,
            autoDelete: false,
            cancellationToken: cancellationToken
        );

        var consumer = new AsyncEventingBasicConsumer(channel);

        consumer.ReceivedAsync += async (sender, eventArgs) =>
        {
            using var scope = serviceProvider.CreateScope();
            var handler = scope.ServiceProvider.GetRequiredService<IMessageConsumer<T>>();

            try
            {
                var body = eventArgs.Body.ToArray();
                var json = Encoding.UTF8.GetString(body);
                var message = JsonSerializer.Deserialize<T>(json);

                await handler.Consume(message, cancellationToken);

                await channel.BasicAckAsync(
                    eventArgs.DeliveryTag,
                    multiple: false,
                    cancellationToken
                );
            }
            catch (Exception ex)
            {
                logger.LogError(
                    ex,
                    "An error occurred while consuming message {Message}",
                    typeof(T).Name
                );

                await channel.BasicNackAsync(
                    eventArgs.DeliveryTag,
                    multiple: false,
                    requeue: false,
                    cancellationToken
                );
            }
        };

        await channel.BasicConsumeAsync(queue, autoAck: false, consumer, cancellationToken);
        //}
        //catch (Exception ex)
        //{
        //    logger.LogError(
        //        ex,
        //        "An error occurred while starting message background service for {Message}",
        //        typeof(T).Name
        //    );
        //}
    }
}
