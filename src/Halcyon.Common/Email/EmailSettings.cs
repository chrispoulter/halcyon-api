namespace Halcyon.Common.Email;

public class EmailSettings
{
    public static string SectionName { get; } = "Email";

    public string SmtpServer { get; set; }

    public int SmtpPort { get; set; }

    public bool SmtpSsl { get; set; }

    public string SmtpUserName { get; set; }

    public string SmtpPassword { get; set; }

    public string NoReplyAddress { get; set; }

    public string SiteUrl { get; set; }
}
