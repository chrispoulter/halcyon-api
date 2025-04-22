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
                        Interface = i,
                        Implementation = t,
                        MessageType = i.GetGenericArguments()[0],
                    })
            )
            .ToList();

        foreach (var entry in consumers)
        {
            var hostedService = typeof(MessageBackgroundService<>).MakeGenericType(
                entry.MessageType
            );
            builder.Services.AddSingleton(typeof(IHostedService), hostedService);
            builder.Services.AddScoped(entry.Interface, entry.Implementation);
        }

        return builder;
    }
}
