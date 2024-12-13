using System.Net;
using System.Net.Http.Json;
using Halcyon.Api.Features.Account.ForgotPassword;

namespace Halcyon.Api.Tests.Features.Account.ForgotPassword;

public class ForgotPasswordEndpointTests(TestWebApplicationFactory factory) : BaseTest(factory)
{
    private const string _requestUri = "/account/forgot-password";

    [Fact]
    public async Task ForgotPassword_ShouldNotSendForgotPasswordEmail_WhenUserNotFound()
    {
        var request = CreateForgotPasswordRequest();

        var response = await _client.PutAsJsonAsync(_requestUri, request);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var eventPublished = await _testHarness.Published.Any<ResetPasswordRequestedEvent>();
        Assert.False(eventPublished, $"{nameof(ResetPasswordRequestedEvent)} published");
    }

    [Fact]
    public async Task ForgotPassword_SendResetPasswordEmail_WhenUserFound()
    {
        var user = await CreateTestUserAsync();
        var request = CreateForgotPasswordRequest(user.EmailAddress);

        var response = await _client.PutAsJsonAsync(_requestUri, request);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var eventPublished = await _testHarness.Published.Any<ResetPasswordRequestedEvent>();
        Assert.True(eventPublished, $"{nameof(ResetPasswordRequestedEvent)} not published");
    }

    private static ForgotPasswordRequest CreateForgotPasswordRequest(string emailAddress = null) =>
        new() { EmailAddress = emailAddress ?? $"{Guid.NewGuid()}@example.com" };
}
