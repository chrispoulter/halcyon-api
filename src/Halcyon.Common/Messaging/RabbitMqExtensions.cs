using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;

namespace Halcyon.Common.Messaging;

public static class RabbitMqExtensions
{
    public static IHostApplicationBuilder AddRabbitMq(
        this IHostApplicationBuilder builder,
        string connectionName,
        Assembly assembly
    )
    {
        builder.AddRabbitMQClient("RabbitMq");
        builder.Services.AddSingleton<IMessagePublisher, MessagePublisher>();

        var consumers = assembly
            .GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract)
            .SelectMany(t =>
                t.GetInterfaces()
                    .Where(i =>
                        i.IsGenericType
                        && i.GetGenericTypeDefinition() == typeof(IMessageConsumer<>)
                    )
                    .Select(i => (ConcreteType: t, ConsumedType: i.GetGenericArguments()[0]))
            );

        foreach (var (ConcreteType, ConsumedType) in consumers)
        {
            var backgroundService = typeof(MessageBackgroundService<,>).MakeGenericType(
                ConsumedType,
                ConcreteType
            );

            builder.Services.AddScoped(ConcreteType);
            builder.Services.AddSingleton(typeof(IHostedService), backgroundService);
        }

        return builder;
    }

    public static async Task<string> CreateMessageExchange<TMessage>(
        this IChannel channel,
        CancellationToken cancellationToken
    )
    {
        var messageExchange = typeof(TMessage).FullName;
        await CreateExchange(channel, messageExchange, cancellationToken);

        return messageExchange;
    }

    public static async Task<string> CreateConsumerQueue<TMessage, TConsumer>(
        this IChannel channel,
        CancellationToken cancellationToken
    )
    {
        var messageExchange = typeof(TMessage).FullName;
        await CreateExchange(channel, messageExchange, cancellationToken);

        var consumerQueue = typeof(TConsumer).FullName;
        await CreateAndBindQueue(channel, consumerQueue, messageExchange, cancellationToken);

        return consumerQueue;
    }

    private static async Task CreateExchange(
        IChannel channel,
        string exchange,
        CancellationToken cancellationToken = default
    )
    {
        await channel.ExchangeDeclareAsync(
            exchange,
            ExchangeType.Fanout,
            durable: true,
            autoDelete: false,
            cancellationToken: cancellationToken
        );
    }

    private static async Task CreateAndBindQueue(
        IChannel channel,
        string queue,
        string exchange,
        CancellationToken cancellationToken = default
    )
    {
        await channel.QueueDeclareAsync(
            queue,
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
    }
}
