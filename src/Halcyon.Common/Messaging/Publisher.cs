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

        var queueName = MessagingExtensions.GetQueueName<T>();

        await channel.QueueDeclareAsync(
            queue: queueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null,
            cancellationToken: cancellationToken
        );

        foreach (var message in messages)
        {
            var messageBodyBytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));
            var props = new BasicProperties { ContentType = "application/json", Persistent = true };

            await channel.BasicPublishAsync(
                exchange: string.Empty,
                routingKey: queueName,
                mandatory: true,
                basicProperties: props,
                body: messageBodyBytes,
                cancellationToken: cancellationToken
            );
        }
    }
}
