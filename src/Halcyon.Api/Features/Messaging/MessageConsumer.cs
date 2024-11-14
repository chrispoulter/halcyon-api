using Halcyon.Api.Core.Database;
using Halcyon.Api.Data;
using MassTransit;
using Microsoft.AspNetCore.SignalR;

namespace Halcyon.Api.Features.Messaging;

public class MessageConsumer(IHubContext<MessageHub, IMessageClient> messageHubContext)
    : IConsumer<Batch<EntityChangedEvent>>
{
    public async Task Consume(ConsumeContext<Batch<EntityChangedEvent>> context)
    {
        var batch = context.Message.DistinctBy(d => new
        {
            d.Message.Id,
            d.Message.ChangeType,
            d.Message.Entity,
        });

        foreach (var item in batch)
        {
            var message = item.Message;

            switch (message.Entity)
            {
                case nameof(User):
                    var groups = new[]
                    {
                        MessageHub.GetGroupForRole(Role.SystemAdministrator),
                        MessageHub.GetGroupForRole(Role.UserAdministrator),
                        MessageHub.GetGroupForUser(message.Id),
                    };

                    await messageHubContext
                        .Clients.Groups(groups)
                        .ReceiveMessage(message, context.CancellationToken);

                    break;
            }
        }
    }
}
