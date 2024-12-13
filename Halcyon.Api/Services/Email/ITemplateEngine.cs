namespace Halcyon.Api.Services.Email;

public interface ITemplateEngine
{
    Task<Tuple<string, string>> RenderTemplateAsync(
        string template,
        dynamic model,
        CancellationToken cancellationToken = default
    );
}
