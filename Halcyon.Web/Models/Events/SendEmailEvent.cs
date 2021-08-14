using Halcyon.Web.Services.Email;
using System.Collections.Generic;

namespace Halcyon.Web.Models.Events
{
    public class SendEmailEvent
    {
        public string EmailAddress { get; set; }

        public EmailTemplate Template { get; set; }

        public Dictionary<string, string> Context { get; set; }
    }
}