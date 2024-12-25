namespace Halcyon.Api.Common.Email;

public interface ITemplateEngine
{
    Task<Tuple<string, string>> RenderTemplateAsync(
        string template,
        dynamic model,
        CancellationToken cancellationToken = default
    );
}
