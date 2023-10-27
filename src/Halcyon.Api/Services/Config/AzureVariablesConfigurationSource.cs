namespace Halcyon.Api.Services.Config;
public class AzureVariablesConfigurationSource : IConfigurationSource
{
    public IConfigurationProvider Build(IConfigurationBuilder builder)
        => new AzureVariablesConfigurationProvider();
}