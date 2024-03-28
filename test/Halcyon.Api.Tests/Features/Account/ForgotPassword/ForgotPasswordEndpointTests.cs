using System.Net;
using System.Net.Http.Json;
using Halcyon.Api.Features.Account.ForgotPassword;
using Halcyon.Api.Features.Account.SendResetPasswordEmail;

namespace Halcyon.Api.Tests.Features.Account.ForgotPassword;

public class ForgotPasswordEndpointTests : BaseTest
{
    private const string _requestUri = "/account/forgot-password";

    public ForgotPasswordEndpointTests(TestWebApplicationFactory factory)
        : base(factory) { }

    [Fact]
    public async Task ForgotPassword_ShouldNotSendForgotPasswordEmail_WhenUserNotFound()
    {
        var request = CreateForgotPasswordRequest();

        var response = await _client.PutAsJsonAsync(_requestUri, request);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var consumerTestHarness = _testHarness.GetConsumerHarness<SendResetPasswordEmailConsumer>();
        var sendEmailEventConsumed =
            await consumerTestHarness.Consumed.Any<SendResetPasswordEmailEvent>(c =>
                c.Context.Message.To == request.EmailAddress
            );
        Assert.False(sendEmailEventConsumed);
    }

    [Fact]
    public async Task ForgotPassword_SendResetPasswordEmail_WhenUserFound()
    {
        var user = await CreateTestUserAsync();
        var request = CreateForgotPasswordRequest(user.EmailAddress);

        var response = await _client.PutAsJsonAsync(_requestUri, request);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var consumerTestHarness = _testHarness.GetConsumerHarness<SendResetPasswordEmailConsumer>();
        var sendEmailEventConsumed =
            await consumerTestHarness.Consumed.Any<SendResetPasswordEmailEvent>(c =>
                c.Context.Message.To == request.EmailAddress
            );
        Assert.True(sendEmailEventConsumed);
    }

    private static ForgotPasswordRequest CreateForgotPasswordRequest(string? emailAddress = null) =>
        new(
            EmailAddress: emailAddress ?? $"{Guid.NewGuid()}@example.com",
            SiteUrl: "http://localhost:3000"
        );
}
