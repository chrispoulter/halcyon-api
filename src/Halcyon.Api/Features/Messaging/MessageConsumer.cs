using Halcyon.Api.Data;
using Halcyon.Api.Features.Messaging;
using MassTransit;
using Microsoft.AspNetCore.SignalR;

namespace Halcyon.Api.Features.Account.SendResetPasswordEmail;

public class MessageConsumer(IHubContext<MessageHub, IMessageClient> messageHubContext)
    : IConsumer<MessageEvent>
{
    public async Task Consume(ConsumeContext<MessageEvent> context)
    {
        var message = context.Message;

        var groups = new string[]
        {
            $"ROLE_{Role.SystemAdministrator}",
            $"ROLE_{Role.UserAdministrator}",
            $"USER_{message.Id}"
        };

        await messageHubContext
            .Clients.Groups(groups)
            .ReceiveMessage(message, context.CancellationToken);
    }
}
