namespace Halcyon.Web.Services.Config
{
    public class AzureConfigurationSource : IConfigurationSource
    {
        public IConfigurationProvider Build(IConfigurationBuilder builder)
            => new AzureConfigurationProvider();
    }
}
