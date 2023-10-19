namespace Halcyon.Api.Services.Email;

public class EmailMessage
{ 
    public string To { get; set; }

    public string Template { get; set; }

    public Dictionary<string, string> Data { get; set; }
}
