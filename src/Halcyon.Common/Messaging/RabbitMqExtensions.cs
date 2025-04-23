using System.Reflection;
using Microsoft.Extensions.Configuration;
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
        builder.Services.AddSingleton<IConnectionFactory>(
            new ConnectionFactory
            {
                Uri = new(builder.Configuration.GetConnectionString(connectionName)),
                AutomaticRecoveryEnabled = true,
            }
        );

        builder.Services.AddSingleton(sp =>
            sp.GetRequiredService<IConnectionFactory>()
                .CreateConnectionAsync()
                .GetAwaiter()
                .GetResult()
        );

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

    public static async Task CreateExchangeWithDeadLetter(
        this IChannel channel,
        string exchange,
        CancellationToken cancellationToken
    )
    {
        var deadLetterExchange = $"{exchange}.DeadLetter";

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
    }

    public static async Task CreateQueueWithDeadLetter(
        this IChannel channel,
        string exchange,
        string queue,
        CancellationToken cancellationToken
    )
    {
        var deadLetterExchange = $"{exchange}.DeadLetter";
        var deadLetterQueue = $"{queue}.DeadLetter";
        var arguments = new Dictionary<string, object>
        {
            { "x-dead-letter-exchange", deadLetterExchange },
        };

        await channel.QueueDeclareAsync(
            queue,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments,
            cancellationToken: cancellationToken
        );

        await channel.QueueBindAsync(
            queue,
            exchange,
            routingKey: string.Empty,
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
            deadLetterQueue,
            deadLetterExchange,
            routingKey: string.Empty,
            cancellationToken: cancellationToken
        );
    }
}
