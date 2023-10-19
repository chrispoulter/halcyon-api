using System.Reflection;
using System.Text.RegularExpressions;

namespace Halcyon.Api.Services.Email;

public partial class TemplateEngine : ITemplateEngine
{
    public async Task<Tuple<string, string>> RenderTemplateAsync(string template, object data)
    {
        var resource = await ReadEmbeddedResourceAsStringAsync(template);
        var tokenReplacements = ObjectToDictionary(data);
        var html = ReplaceTokensInHtml(resource, tokenReplacements);
        var title = GetHtmlTitle(html);

        return new(html, title);
    }

    private static async Task<string> ReadEmbeddedResourceAsStringAsync(string resource)
    {
        var assembly = Assembly.GetExecutingAssembly();

        var name = assembly.GetManifestResourceNames()
            .Single(str => str.EndsWith(resource));

        using var stream = assembly.GetManifestResourceStream(name);
        using var reader = new StreamReader(stream);

        return await reader.ReadToEndAsync();
    }

    private static string GetHtmlTitle(string html)
    {
        var pattern = @"<title\b[^>]*>(.*?)</title>";
        var match = Regex.Match(html, pattern, RegexOptions.IgnoreCase);

        return match.Success 
            ? match.Groups[1].Value
            : string.Empty;
    }

    public static string ReplaceTokensInHtml(string html, IDictionary<string, string> tokenReplacements)
    {
        foreach (var (token, replacement) in tokenReplacements)
        {
            var pattern = $"{{{{ {token} }}}}";
            html = Regex.Replace(html, pattern, replacement);
        }

        return html;
    }

    public static IDictionary<string, string> ObjectToDictionary(object obj)
    {
        var result = new Dictionary<string, string>();

        foreach (var property in obj.GetType().GetProperties())
        {
            result[property.Name] = property.GetValue(obj).ToString();
        }

        return result;
    }
}
