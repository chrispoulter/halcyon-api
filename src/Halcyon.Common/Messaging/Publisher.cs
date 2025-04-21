using System.Text;
using System.Text.Json;
using RabbitMQ.Client;

namespace Halcyon.Common.Messaging;

public partial class Publisher(IConnectionFactory connectionFactory) : IPublisher
{
    public async Task Publish<T>(IEnumerable<T> messages, CancellationToken cancellationToken)
    {
        var connection = await connectionFactory.CreateConnectionAsync(cancellationToken);
        var channel = await connection.CreateChannelAsync(cancellationToken: cancellationToken);

        var queue = MessagingExtensions.GetQueueName<T>();

        await channel.QueueDeclareAsync(
            queue,
            durable: true,
            exclusive: false,
            autoDelete: false,
            cancellationToken: cancellationToken
        );

        foreach (var message in messages)
        {
            var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));

            var properties = new BasicProperties
            {
                ContentType = "application/json",
                Persistent = true,
            };

            await channel.BasicPublishAsync(
                exchange: string.Empty,
                routingKey: queue,
                mandatory: true,
                properties,
                body,
                cancellationToken: cancellationToken
            );
        }
    }
}
