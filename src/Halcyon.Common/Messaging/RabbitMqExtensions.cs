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
            (_) =>
                new ConnectionFactory
                {
                    Uri = new Uri(builder.Configuration.GetConnectionString(connectionName)),
                    AutomaticRecoveryEnabled = true,
                }
        );

        builder.Services.AddScoped<IMessagePublisher, MessagePublisher>();

        var consumers = assembly
            .GetTypes()
            .Where(t => !t.IsAbstract && !t.IsInterface)
            .SelectMany(t =>
                t.GetInterfaces()
                    .Where(i =>
                        i.IsGenericType
                        && i.GetGenericTypeDefinition() == typeof(IMessageConsumer<>)
                    )
                    .Select(i => new
                    {
                        Implementation = t,
                        MessageType = i.GetGenericArguments()[0],
                    })
            )
            .ToList();

        foreach (var consumer in consumers)
        {
            builder.Services.AddScoped(consumer.Implementation);

            var backgroundService = typeof(MessageBackgroundService<,>).MakeGenericType(
                consumer.MessageType,
                consumer.Implementation
            );

            builder.Services.AddSingleton(typeof(IHostedService), backgroundService);
        }

        return builder;
    }
}
