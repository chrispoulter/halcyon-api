namespace Halcyon.Web.Services.Email
{

    public class EmailMessage
    {
        public EmailMessage()
        {
            To = new List<string>();
            Data = new Dictionary<string, string>();
        }

        public EmailTemplate Template { get; set; }

        public List<string> To { get; set; }

        public string From { get; set; }

        public Dictionary<string, string> Data { get; set; }
    }
}
