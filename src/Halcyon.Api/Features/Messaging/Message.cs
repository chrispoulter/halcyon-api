
namespace Halcyon.Api.Features.Messaging;

public class Message
{
    public string Content { get; set; }

    public int CreatedBy { get; set; }

    public DateTimeOffset CreatedAt { get; set; }
}