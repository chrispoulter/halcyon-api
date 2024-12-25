using System.Reflection;
using System.Text.RegularExpressions;

namespace Halcyon.Api.Common.Email;

public partial class TemplateEngine : ITemplateEngine
{
    public async Task<Tuple<string, string>> RenderTemplateAsync(
        string template,
        dynamic model,
        CancellationToken cancellationToken = default
    )
    {
        var resource = await ReadEmbeddedResourceAsStringAsync(template, cancellationToken);
        var html = Render(resource, model);
        var title = GetHtmlTitle(html);

        return new(html, title);
    }

    private static async Task<string> ReadEmbeddedResourceAsStringAsync(
        string resource,
        CancellationToken cancellationToken
    )
    {
        var assembly = Assembly.GetExecutingAssembly();

        var name = assembly.GetManifestResourceNames().Single(str => str.EndsWith(resource));

        using var stream = assembly.GetManifestResourceStream(name);
        using var reader = new StreamReader(stream);

        return await reader.ReadToEndAsync(cancellationToken);
    }

    private static string Render(string html, dynamic model)
    {
        var pattern = @"\{{(.+?)\}}";

        return Regex.Replace(
            html,
            pattern,
            match =>
            {
                var key = match.Groups[1].Value.Trim();
                object value = model;

                foreach (var k in key.Split('.'))
                {
                    value = value.GetType().GetProperty(k).GetValue(value, null);
                }

                return value.ToString();
            }
        );
    }

    private static string GetHtmlTitle(string html)
    {
        var pattern = @"<title\b[^>]*>(.*?)</title>";
        var match = Regex.Match(html, pattern, RegexOptions.IgnoreCase);

        return match.Success ? match.Groups[1].Value : string.Empty;
    }
}
