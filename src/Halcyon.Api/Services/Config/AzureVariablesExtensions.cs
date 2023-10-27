namespace Halcyon.Api.Services.Config;

public static class AzureVariablesExtensions
{
    public static IConfigurationBuilder AddAzureVariables(this IConfigurationBuilder builder)
    {
        builder.Add(new AzureVariablesConfigurationSource());
        return builder;
    }
}
