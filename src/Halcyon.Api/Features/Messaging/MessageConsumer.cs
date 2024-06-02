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
                    MessageHub.GetGroupForRole(Role.SystemAdministrator),
                    MessageHub.GetGroupForRole(Role.UserAdministrator),
                    MessageHub.GetGroupForUser(message.Id)
                };

                await messageHubContext
                    .Clients.Groups(userGroups)
                    .ReceiveMessage(message, context.CancellationToken);

                break;
        }
    }
}
