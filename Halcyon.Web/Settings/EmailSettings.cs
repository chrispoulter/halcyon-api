namespace Halcyon.Web.Settings
{
    public class EmailSettings
    {
        public string SmtpServer { get; set; }

        public int SmtpPort { get; set; }

        public string SmtpUserName { get; set; }

        public string SmtpPassword { get; set; }

        public string NoReplyAddress { get; set; }
    }
}
