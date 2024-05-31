namespace Halcyon.Api.Features.Messaging;

public class MessageEvent
{
    public int Id { get; set; }

    public string ChangeType { get; set; }

    public string Entity { get; set; }
}
