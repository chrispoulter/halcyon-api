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
}
