using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Halcyon.Api.Tests;

public class TestAuthenticationOptions : AuthenticationSchemeOptions { }

public class TestAuthenticationHandler : AuthenticationHandler<TestAuthenticationOptions>
{
    public const string AuthenticationScheme = "TestScheme";

    public const string UserId = "UserId";

    public TestAuthenticationHandler(
        IOptionsMonitor<TestAuthenticationOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder
    )
        : base(options, logger, encoder) { }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Context.Request.Headers.TryGetValue(UserId, out var userId))
        {
            return Task.FromResult(AuthenticateResult.NoResult());
        }

        var claims = new[] { new Claim(ClaimTypes.Name, userId.ToString()) };
        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);

        var result = AuthenticateResult.Success(ticket);
        return Task.FromResult(result);
    }
}
