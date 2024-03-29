namespace Halcyon.Api.Services.Messaging;

public class MessagingSettings
{
    public static string SectionName { get; } = "Messaging";

    public string EndpointPrefix { get; set; }

    public MessagingProvider Provider { get; set; }

    public string ConnectionString { get; set; }
}
