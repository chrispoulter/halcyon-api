namespace Halcyon.Web.Settings
{
    public class EventSettings
    {
        public string StorageConnectionString { get; set; }

        public int PollingInterval { get; set; }

        public int BatchSize { get; set; }
    }
}
