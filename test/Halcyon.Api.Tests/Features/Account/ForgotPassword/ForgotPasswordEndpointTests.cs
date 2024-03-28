using System.Net;
using System.Net.Http.Json;
using Halcyon.Api.Features.Account.ForgotPassword;
using Halcyon.Api.Features.Account.SendResetPasswordEmail;
using Halcyon.Api.Services.Email;
using Moq;

namespace Halcyon.Api.Tests.Features.Account.ForgotPassword;

public class ForgotPasswordEndpointTests : IClassFixture<TestWebApplicationFactory>
{
    private const string _requestUri = "/account/forgot-password";

    private readonly TestWebApplicationFactory _factory;

    public ForgotPasswordEndpointTests(TestWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task ForgotPassword_ShouldNotSendForgotPasswordEmail_WhenUserNotFound()
    {
        var request = CreateForgotPasswordRequest();

        var client = _factory.CreateClient();
        var response = await client.PutAsJsonAsync(_requestUri, request);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var consumerTestHarness = _factory.GetConsumerTestHarness<SendResetPasswordEmailConsumer>();
        Assert.False(
            await consumerTestHarness.Consumed.Any<SendResetPasswordEmailEvent>(c =>
                c.Context.Message.To == request.EmailAddress
            )
        );

        _factory.MockEmailSender.Verify(
            e =>
                e.SendEmailAsync(
                    It.Is<EmailMessage>(a => a.To == request.EmailAddress),
                    It.IsAny<CancellationToken>()
                ),
            Times.Never()
        );
    }

    [Fact]
    public async Task ForgotPassword_SendResetPasswordEmail_WhenUserFound()
    {
        var user = await _factory.CreateTestUserAsync();
        var request = CreateForgotPasswordRequest(user.EmailAddress);

        var client = _factory.CreateClient();
        var response = await client.PutAsJsonAsync(_requestUri, request);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var consumerTestHarness = _factory.GetConsumerTestHarness<SendResetPasswordEmailConsumer>();
        Assert.True(
            await consumerTestHarness.Consumed.Any<SendResetPasswordEmailEvent>(c =>
                c.Context.Message.To == user.EmailAddress
            )
        );

        _factory.MockEmailSender.Verify(
            e =>
                e.SendEmailAsync(
                    It.Is<EmailMessage>(a => a.To == request.EmailAddress),
                    It.IsAny<CancellationToken>()
                ),
            Times.Once()
        );
    }

    private static ForgotPasswordRequest CreateForgotPasswordRequest(string? emailAddress = null) =>
        new()
        {
            EmailAddress = emailAddress ?? $"{Guid.NewGuid()}@example.com",
            SiteUrl = "http://localhost:3000"
        };
}
