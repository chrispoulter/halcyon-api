﻿namespace Halcyon.Web.Services.Email
{

    public class EmailMessage
    {
        public EmailTemplate Template { get; set; }

        public List<string> To { get; set; } = new();

        public string From { get; set; }

        public Dictionary<string, string> Data { get; set; } = new();
    }
}
