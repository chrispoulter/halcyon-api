namespace Halcyon.Api.Services.Email;

public record EmailMessage(string Template, string To, object Data) { }
