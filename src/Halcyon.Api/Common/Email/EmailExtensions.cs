namespace Halcyon.Api.Common.Email;

public static class EmailExtensions
{
    public static IHostApplicationBuilder AddEmailServices(this IHostApplicationBuilder builder)
    {
        var emailConfig = builder.Configuration.GetSection(EmailSettings.SectionName);
        builder.Services.Configure<EmailSettings>(emailConfig);
        builder.Services.AddScoped<IEmailService, EmailService>();
        builder.Services.AddSingleton<ITemplateEngine, TemplateEngine>();

        return builder;
    }
}
