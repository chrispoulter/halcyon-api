using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Halcyon.Common.Messaging;

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
            "Message service for {Message} with {Consumer} started",
            typeof(TMessage).Name,
            typeof(TConsumer).Name
        );

        using var channel = await connection.CreateChannelAsync(
            cancellationToken: cancellationToken
        );

        var queue = await GetQueue(channel, cancellationToken);

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
                    "An error occurred while consuming message {Message} with {Consumer}",
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

        await channel.BasicConsumeAsync(queue, autoAck: false, consumer, cancellationToken);

        await Task.Delay(Timeout.Infinite, cancellationToken);
    }

    private static async Task<string> GetQueue(
        IChannel channel,
        CancellationToken cancellationToken
    )
    {
        var exchange = typeof(TMessage).FullName;
        var deadLetterExchange = $"{exchange}.DeadLetter";

        var queue = typeof(TConsumer).FullName;
        var deadLetterQueue = $"{queue}.DeadLetter";

        var arguments = new Dictionary<string, object>
        {
            { "x-dead-letter-exchange", deadLetterExchange },
        };

        await channel.ExchangeDeclareAsync(
            exchange,
            ExchangeType.Fanout,
            durable: true,
            autoDelete: false,
            cancellationToken: cancellationToken
        );

        await channel.ExchangeDeclareAsync(
            deadLetterExchange,
            ExchangeType.Fanout,
            durable: true,
            autoDelete: false,
            cancellationToken: cancellationToken
        );

        await channel.QueueDeclareAsync(
            queue,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments,
            cancellationToken: cancellationToken
        );

        await channel.QueueDeclareAsync(
            deadLetterQueue,
            durable: true,
            exclusive: false,
            autoDelete: false,
            cancellationToken: cancellationToken
        );

        await channel.QueueBindAsync(
            queue,
            exchange,
            routingKey: string.Empty,
            cancellationToken: cancellationToken
        );

        await channel.QueueBindAsync(
            deadLetterQueue,
            deadLetterExchange,
            routingKey: string.Empty,
            cancellationToken: cancellationToken
        );

        return queue;
    }
}
