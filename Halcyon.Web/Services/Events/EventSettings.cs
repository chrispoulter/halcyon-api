namespace Halcyon.Web.Services.Events
{
    public class EventSettings
    {
        public string StorageConnectionString { get; set; }

        public int PollingInterval { get; set; }

        public int BatchSize { get; set; }
    }
}
