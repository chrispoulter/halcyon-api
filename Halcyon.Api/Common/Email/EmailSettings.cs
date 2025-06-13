using System.Data.Common;

namespace Halcyon.Api.Common.Email;

public class EmailSettings
{
    public static string SectionName { get; } = "Email";

    public string SmtpServer { get; set; } = null!;

    public int SmtpPort { get; set; }

    public bool SmtpSsl { get; set; }

    public string SmtpUserName { get; set; } = null!;

    public string SmtpPassword { get; set; } = null!;

    public string NoReplyAddress { get; set; } = null!;

    public string SiteUrl { get; set; } = null!;

    internal void ParseConnectionString(string connectionString)
    {
        var connectionStringBuilder = new DbConnectionStringBuilder
        {
            ConnectionString = connectionString,
        };

        if (connectionStringBuilder.TryGetValue("Host", out var host))
        {
            SmtpServer = host?.ToString() ?? string.Empty;
        }

        if (connectionStringBuilder.TryGetValue("Port", out var port))
        {
            if (int.TryParse(port?.ToString(), out var portValue))
            {
                SmtpPort = portValue;
            }
        }

        if (connectionStringBuilder.TryGetValue("Ssl", out var ssl))
        {
            SmtpSsl = ssl?.ToString() == "true";
        }

        if (connectionStringBuilder.TryGetValue("UserName", out var username))
        {
            SmtpUserName = username?.ToString() ?? string.Empty;
        }

        if (connectionStringBuilder.TryGetValue("Password", out var password))
        {
            SmtpPassword = password?.ToString() ?? string.Empty;
        }
    }
}
