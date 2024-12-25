namespace Halcyon.Api.Common.Email;

public record EmailMessage(string Template, string To, object Data) { }
