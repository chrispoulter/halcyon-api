namespace Halcyon.Api.Services.Email
{
    public class EmailMessage
    {
        public EmailTemplate Template { get; set; }

        public string To { get; set; }

        public string From { get; set; }

        public Dictionary<string, string> Data { get; set; }
    }
}
