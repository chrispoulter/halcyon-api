namespace Halcyon.Api.Services.Email;

public class EmailSettings
{
    public static string SectionName { get; } = "Email";

    public string SmtpServer { get; set; }

    public int SmtpPort { get; set; }

    public string SmtpUserName { get; set; }

    public string SmtpPassword { get; set; }

    public string NoReplyAddress { get; set; }
}
