using System.Text;
using System.Text.Json;
using RabbitMQ.Client;

namespace Halcyon.Common.Messaging;

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

        var messageExchange = await ConfigureRabbitMq<TMessage>(channel, cancellationToken);

        foreach (var message in messages)
        {
            var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));

            var properties = new BasicProperties
            {
                ContentType = "application/json",
                Persistent = true,
            };

            await channel.BasicPublishAsync(
                messageExchange,
                routingKey: string.Empty,
                mandatory: true,
                properties,
                body,
                cancellationToken: cancellationToken
            );
        }
    }

    private static async Task<string> ConfigureRabbitMq<TMessage>(
        IChannel channel,
        CancellationToken cancellationToken
    )
    {
        var messageExchange = typeof(TMessage).FullName;

        await channel.ExchangeDeclareAsync(
            messageExchange,
            ExchangeType.Fanout,
            durable: true,
            autoDelete: false,
            cancellationToken: cancellationToken
        );

        return messageExchange;
    }
}
