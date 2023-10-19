using System.Reflection;
using System.Text.RegularExpressions;

namespace Halcyon.Api.Services.Email;

public partial class TemplateEngine : ITemplateEngine
{
    public async Task<Tuple<string, string>> RenderTemplateAsync(string template, IDictionary<string, object> data)
    {
        var html = await ReadResourceAsync(template);
        var title = GetHtmlTitle(html);

        foreach (var entry in data)
        {
            html = html.Replace($"{{{{ {entry.Key} }}}}", entry.Value.ToString());
            title = title.Replace($"{{{{ {entry.Key} }}}}", entry.Value.ToString());
        }

        return new(html, title);
    }

    private async Task<string> ReadResourceAsync(string resource)
    {
        var assembly = Assembly.GetExecutingAssembly();

        var name = assembly.GetManifestResourceNames()
            .Single(str => str.EndsWith(resource));

        using var stream = assembly.GetManifestResourceStream(name);
        using var reader = new StreamReader(stream);

        return await reader.ReadToEndAsync();
    }

    private string GetHtmlTitle(string html)
    {
        var match = HtmlTitleRegex().Match(html);

        return match.Success
            ? match.Groups[1].Value
            : string.Empty;
    }

    [GeneratedRegex("<title>\\s*(.+?)\\s*</title>")]
    private static partial Regex HtmlTitleRegex();
}
