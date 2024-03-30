namespace Halcyon.Api.Services.Messaging;

public class MessagingSettings
{
    public static string SectionName { get; } = "Messaging";

    public MessagingProvider? Provider { get; set; }

    public string ConnectionString { get; set; }

    public string Prefix { get; set; }
}
