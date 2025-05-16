using System.Text;
using System.Text.Json;
using RabbitMQ.Client;

namespace Halcyon.Api.Common.Messaging;

public partial class MessagePublisher(IConnection connection) : IMessagePublisher
{
    public async Task Publish<TMessage>(
        IEnumerable<TMessage> messages,
        CancellationToken cancellationToken
    )
    {
        using var channel = await connection.CreateChannelAsync(
            cancellationToken: cancellationToken
        );

        var exchange = await channel.CreateMessageExchange<TMessage>(cancellationToken);

        foreach (var message in messages)
        {
            var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));

            var properties = new BasicProperties
            {
                ContentType = "application/json",
                Persistent = true,
            };

            await channel.BasicPublishAsync(
                exchange,
                routingKey: string.Empty,
                mandatory: true,
                properties,
                body,
                cancellationToken: cancellationToken
            );
        }
    }
}
