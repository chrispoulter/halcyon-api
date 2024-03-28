namespace Halcyon.Api.Features.Account.SendResetPasswordEmail;

public record SendResetPasswordEmailEvent(string To, Guid? PasswordResetToken, string SiteUrl);
