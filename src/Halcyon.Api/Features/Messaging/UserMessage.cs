namespace Halcyon.Api.Features.Messaging;

public record UserMessage(string Sender, string Content, DateTime SentTime);
