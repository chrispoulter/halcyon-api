using System.Reflection;
using System.Text.RegularExpressions;
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

        builder.Services.AddTransient<IPublisher, Publisher>();

        var consumers = assembly
            .GetTypes()
            .Where(t => !t.IsAbstract && !t.IsInterface)
            .SelectMany(t =>
                t.GetInterfaces()
                    .Where(i =>
                        i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IConsumer<>)
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
            var hostedService = typeof(ConsumerHostedService<>).MakeGenericType(entry.MessageType);
            builder.Services.AddSingleton(typeof(IHostedService), hostedService);
            builder.Services.AddTransient(entry.Interface, entry.Implementation);
        }

        return builder;
    }

    public static string GetQueueName<T>()
    {
        var typeName = typeof(T).FullName;
        return Regex.Replace(typeName.Replace(".", ""), "(?<!^)([A-Z])", "-$1").ToLowerInvariant();
    }
}
