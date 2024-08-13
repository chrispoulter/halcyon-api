namespace Halcyon.Api.Core.Email;

public class EmailMessage
{
    public string To { get; set; }

    public string Template { get; set; }

    public object Data { get; set; }
}
