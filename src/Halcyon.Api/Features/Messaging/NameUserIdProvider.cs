using System.Security.Claims;
using Microsoft.AspNetCore.SignalR;

namespace Halcyon.Api.Features.Messaging;

public class NameUserIdProvider : IUserIdProvider
{
    public virtual string GetUserId(HubConnectionContext connection) =>
        connection.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
}
