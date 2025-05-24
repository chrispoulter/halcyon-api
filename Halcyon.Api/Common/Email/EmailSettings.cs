using System.Data.Common;

namespace Halcyon.Api.Common.Email;

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

    internal void ParseConnectionString(string connectionString)
    {
        var connectionStringBuilder = new DbConnectionStringBuilder
        {
            ConnectionString = connectionString,
        };

        if (connectionStringBuilder.TryGetValue("Host", out var host))
        {
            SmtpServer = host?.ToString();
        }

        if (connectionStringBuilder.TryGetValue("Port", out var port))
        {
            if (int.TryParse(port.ToString(), out var portValue))
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
            SmtpUserName = username?.ToString();
        }

        if (connectionStringBuilder.TryGetValue("Password", out var password))
        {
            SmtpPassword = password?.ToString();
        }
    }
}
