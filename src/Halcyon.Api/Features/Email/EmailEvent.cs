using Halcyon.Api.Features.Email.Templates;

namespace Halcyon.Api.Features.Email
{
    public class EmailEvent
    {
        public EmailTemplate Template { get; set; }

        public string To { get; set; }

        public Dictionary<string, string> Data { get; set; }
    }
}
