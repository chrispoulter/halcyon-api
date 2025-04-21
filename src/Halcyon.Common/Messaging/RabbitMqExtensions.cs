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

        builder.Services.AddTransient<IPublisher, Publisher>();




        //var consumers = assembly
        //    .GetTypes()
        //    .Where(t => !t.IsAbstract && !t.IsInterface)
        //    .SelectMany(t =>
        //        t.GetInterfaces()
        //            .Where(i =>
        //                i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IConsumer<>)
        //            )
        //            .Select(i => new
        //            {
        //                Interface = i,
        //                Implementation = t,
        //                MessageType = i.GetGenericArguments()[0],
        //            })
        //    )
        //    .ToList();

        //foreach (var entry in consumers)
        //{
        //    builder.Services.AddSingleton(entry.Interface, entry.Implementation);

        //    var hostedService = typeof(ConsumerBackgroundService<>).MakeGenericType(
        //        entry.MessageType
        //    );

        //    builder.Services.AddSingleton(typeof(IHostedService), hostedService);
        //}

        return builder;
    }
}
