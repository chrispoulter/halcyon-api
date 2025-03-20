namespace Halcyon.Api.Common.Email;

public class EmailSettings
{
    public static string SectionName { get; } = "Email";

    public string NoReplyAddress { get; set; }

    public string CdnUrl { get; set; }
}
