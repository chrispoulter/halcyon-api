using Microsoft.Extensions.Configuration.EnvironmentVariables;

namespace Halcyon.Api.Services.Config;

public class AzureVariablesConfigurationProvider : EnvironmentVariablesConfigurationProvider
{
    public override bool TryGet(string key, out string value)
    {
        var newKey = key.Replace(".", "_");
        return base.TryGet(newKey, out value);
    }
}

