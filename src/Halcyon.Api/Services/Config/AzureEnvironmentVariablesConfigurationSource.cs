namespace Halcyon.Api.Services.Config;
public class AzureEnvironmentVariablesConfigurationSource : IConfigurationSource
{
    public IConfigurationProvider Build(IConfigurationBuilder builder)
        => new AzureEnvironmentVariablesConfigurationProvider();
}