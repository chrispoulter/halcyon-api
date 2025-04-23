using System.Text;
using System.Text.Json;
using RabbitMQ.Client;

namespace Halcyon.Common.Messaging;

public partial class MessagePublisher(IConnection connection) : IMessagePublisher
{
    public async Task Publish<T>(IEnumerable<T> messages, CancellationToken cancellationToken)
    {
        using var channel = await connection.CreateChannelAsync(
            cancellationToken: cancellationToken
        );

        var exchange = typeof(T).FullName;

        await ConfigureRabbitMq(channel, exchange, cancellationToken);

        foreach (var message in messages)
        {
            var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));

            var properties = new BasicProperties
            {
                ContentType = "application/json",
                Persistent = true,
            };

            await channel.BasicPublishAsync(
                exchange: exchange,
                routingKey: string.Empty,
                mandatory: true,
                properties,
                body,
                cancellationToken: cancellationToken
            );
        }
    }

    private static async Task ConfigureRabbitMq(
        IChannel channel,
        string exchange,
        CancellationToken cancellationToken
    )
    {
        await channel.ExchangeDeclareAsync(
            exchange,
            ExchangeType.Fanout,
            durable: true,
            autoDelete: false,
            cancellationToken: cancellationToken
        );
    }
}
