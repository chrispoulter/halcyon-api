using Halcyon.Api.Data;
using Halcyon.Api.Features.Messaging;
using MassTransit;
using Microsoft.AspNetCore.SignalR;

namespace Halcyon.Api.Features.Account.SendResetPasswordEmail;

public class MessageConsumer(IHubContext<MessageHub, IMessageClient> messageHubContext)
    : IConsumer<EntityChangedEvent>
{
    public async Task Consume(ConsumeContext<EntityChangedEvent> context)
    {
        var message = context.Message;

        switch (message.Entity)
        {
            case nameof(User):
                var userGroups = new[]
                {
                    $"ROLE_{Role.SystemAdministrator}",
                    $"ROLE_{Role.UserAdministrator}",
                    $"USER_{message.Id}"
                };

                await messageHubContext
                    .Clients.Groups(userGroups)
                    .ReceiveMessage(message, context.CancellationToken);

                break;
        }
    }
}
