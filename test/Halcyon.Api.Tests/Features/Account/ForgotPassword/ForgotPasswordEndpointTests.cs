using System.Net;
using System.Net.Http.Json;
using Halcyon.Api.Features.Account.ForgotPassword;
using Halcyon.Api.Features.Account.SendResetPasswordEmail;

namespace Halcyon.Api.Tests.Features.Account.ForgotPassword;

public class ForgotPasswordEndpointTests : IClassFixture<TestWebApplicationFactory>
{
    private const string RequestUri = "/account/forgot-password";

    private readonly TestWebApplicationFactory factory;

    public ForgotPasswordEndpointTests(TestWebApplicationFactory factory)
    {
        this.factory = factory;
    }

    [Fact]
    public async Task ForgotPassword_ShouldNotSendForgotPasswordEmail_WhenUserNotFound()
    {
        var request = CreateForgotPasswordRequest();

        var client = factory.CreateClient();
        var response = await client.PutAsJsonAsync(RequestUri, request);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var consumerTestHarness = factory.GetConsumerTestHarness<SendResetPasswordEmailConsumer>();
        Assert.False(
            await consumerTestHarness.Consumed.Any<SendResetPasswordEmailEvent>(c =>
                c.Context.Message.To == request.EmailAddress
            )
        );
    }

    [Fact]
    public async Task ForgotPassword_SendResetPasswordEmail_WhenUserFound()
    {
        var user = await factory.CreateTestUserAsync();
        var request = CreateForgotPasswordRequest(user.EmailAddress);

        var client = factory.CreateClient();
        var response = await client.PutAsJsonAsync(RequestUri, request);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var consumerTestHarness = factory.GetConsumerTestHarness<SendResetPasswordEmailConsumer>();
        Assert.True(
            await consumerTestHarness.Consumed.Any<SendResetPasswordEmailEvent>(c =>
                c.Context.Message.To == user.EmailAddress
            )
        );
    }

    private static ForgotPasswordRequest CreateForgotPasswordRequest(string? emailAddress = null) =>
        new()
        {
            EmailAddress = emailAddress ?? $"{Guid.NewGuid()}@example.com",
            SiteUrl = "http://localhost:3000"
        };
}
