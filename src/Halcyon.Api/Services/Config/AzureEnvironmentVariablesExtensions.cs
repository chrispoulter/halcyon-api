namespace Halcyon.Api.Services.Config;

public static class AzureEnvironmentVariablesExtensions
{
    public static IConfigurationBuilder AddAzureEnvironmentVariables(this IConfigurationBuilder builder)
    {
        builder.Add(new AzureEnvironmentVariablesConfigurationSource());
        return builder;
    }
}
