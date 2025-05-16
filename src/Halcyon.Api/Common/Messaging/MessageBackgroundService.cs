using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Halcyon.Api.Common.Messaging;

public class MessageBackgroundService<TMessage, TConsumer>(
    IConnection connection,
    IServiceProvider serviceProvider,
    ILogger<MessageBackgroundService<TMessage, TConsumer>> logger
) : BackgroundService
    where TConsumer : IMessageConsumer<TMessage>
{
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Message service for {MessageType} with {Consumer} started",
            typeof(TMessage).Name,
            typeof(TConsumer).Name
        );

        using var channel = await connection.CreateChannelAsync(
            cancellationToken: cancellationToken
        );

        var consumer = new AsyncEventingBasicConsumer(channel);

        consumer.ReceivedAsync += async (model, ea) =>
        {
            try
            {
                using var scope = serviceProvider.CreateScope();
                var handler = scope.ServiceProvider.GetRequiredService<TConsumer>();

                var body = ea.Body.ToArray();
                var message = JsonSerializer.Deserialize<TMessage>(body);

                await handler.Consume(message, cancellationToken);
                await channel.BasicAckAsync(ea.DeliveryTag, multiple: false, cancellationToken);
            }
            catch (Exception ex)
            {
                logger.LogError(
                    ex,
                    "An error occurred while consuming message {MessageType} with {Consumer}",
                    typeof(TMessage).Name,
                    typeof(TConsumer).Name
                );

                await channel.BasicNackAsync(
                    ea.DeliveryTag,
                    multiple: false,
                    requeue: false,
                    cancellationToken
                );
            }
        };

        var queue = await channel.CreateConsumerQueue<TMessage, TConsumer>(cancellationToken);

        await channel.BasicConsumeAsync(queue, autoAck: false, consumer, cancellationToken);

        await Task.Delay(Timeout.Infinite, cancellationToken);
    }
}
