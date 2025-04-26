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

        var consumerQueue = await ConfigureRabbitMq(channel, cancellationToken);

        await channel.BasicConsumeAsync(consumerQueue, autoAck: false, consumer, cancellationToken);

        await Task.Delay(Timeout.Infinite, cancellationToken);
    }

    private static async Task<string> ConfigureRabbitMq(
        IChannel channel,
        CancellationToken cancellationToken
    )
    {
        var messageExchange = typeof(TMessage).FullName;

        await channel.ExchangeDeclareAsync(
            messageExchange,
            ExchangeType.Fanout,
            durable: true,
            autoDelete: false,
            cancellationToken: cancellationToken
        );

        var consumerExchange = typeof(TConsumer).FullName;

        await channel.ExchangeDeclareAsync(
            consumerExchange,
            ExchangeType.Fanout,
            durable: true,
            autoDelete: false,
            cancellationToken: cancellationToken
        );

        await channel.ExchangeBindAsync(
            destination: consumerExchange,
            source: messageExchange,
            routingKey: string.Empty,
            cancellationToken: cancellationToken
        );

        var consumerDeadLetterExchange = $"{consumerExchange}.DeadLetter";

        await channel.ExchangeDeclareAsync(
            exchange: consumerDeadLetterExchange,
            ExchangeType.Fanout,
            durable: true,
            autoDelete: false,
            cancellationToken: cancellationToken
        );

        var consumerQueue = typeof(TConsumer).FullName;

        var consumerQueueArguments = new Dictionary<string, object>
        {
            { "x-dead-letter-exchange", consumerDeadLetterExchange },
        };

        await channel.QueueDeclareAsync(
            consumerQueue,
            durable: true,
            exclusive: false,
            autoDelete: false,
            consumerQueueArguments,
            cancellationToken: cancellationToken
        );

        await channel.QueueBindAsync(
            consumerQueue,
            consumerExchange,
            routingKey: string.Empty,
            cancellationToken: cancellationToken
        );

        var consumerDeadLetterQueue = $"{consumerQueue}.DeadLetter";

        await channel.QueueDeclareAsync(
            consumerDeadLetterQueue,
            durable: true,
            exclusive: false,
            autoDelete: false,
            consumerQueueArguments,
            cancellationToken: cancellationToken
        );

        await channel.QueueBindAsync(
            consumerDeadLetterQueue,
            consumerDeadLetterExchange,
            routingKey: string.Empty,
            cancellationToken: cancellationToken
        );

        return consumerQueue;
    }
}
