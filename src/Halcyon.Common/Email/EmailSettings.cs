using System.Data.Common;

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

    public string CdnUrl { get; set; }

    internal void ParseConnectionString(string connectionString)
    {
        var connectionStringBuilder = new DbConnectionStringBuilder
        {
            ConnectionString = connectionString,
        };

        connectionStringBuilder.TryGetValue("Endpoint", out var endpoint);
        connectionStringBuilder.TryGetValue("UserName", out var username);
        connectionStringBuilder.TryGetValue("Password", out var password);

        var uri = new Uri(endpoint.ToString());
        SmtpServer = uri.Host;
        SmtpPort = uri.Port;
        SmtpSsl = uri.Scheme.EndsWith('s');

        SmtpUserName = username?.ToString();
        SmtpPassword = password?.ToString();
    }
}
