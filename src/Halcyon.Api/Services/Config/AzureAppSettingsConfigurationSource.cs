namespace Halcyon.Api.Services.Config;

public class AzureAppSettingsConfigurationSource : IConfigurationSource
{
    public IConfigurationProvider Build(IConfigurationBuilder builder)
        => new AzureAppSettingsConfigurationProvider();
}