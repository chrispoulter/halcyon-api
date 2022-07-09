using Microsoft.Extensions.Configuration.EnvironmentVariables;

namespace Halcyon.Web.Services.Config
{
    public class AzureConfigurationProvider : EnvironmentVariablesConfigurationProvider
    {
        public override bool TryGet(string key, out string value)
        {
            var newKey = key.Replace(".", "_");
            return base.TryGet(newKey, out value);
        }
    }
}
