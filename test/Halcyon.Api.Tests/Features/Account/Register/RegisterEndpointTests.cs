using System.Net;
using System.Net.Http.Json;
using Halcyon.Api.Core.Web;
using Halcyon.Api.Features.Account.Register;
using Microsoft.AspNetCore.Mvc;

namespace Halcyon.Api.Tests.Features.Account.Register;

public class RegisterEndpointTests(TestWebApplicationFactory factory) : BaseTest(factory)
{
    private const string _requestUri = "/account/register";

    [Fact]
    public async Task Register_ShouldReturnBadRequest_WhenDuplicateEmailAddress()
    {
        var user = await CreateTestUserAsync();
        var request = CreateRegisterRequest(user.EmailAddress);

        var response = await _client.PostAsJsonAsync(_requestUri, request);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        Assert.NotNull(result);
        Assert.Equal("User name is already taken.", result.Title);
    }

    [Fact]
    public async Task Register_ShouldCreateNewUser_WhenRequestValid()
    {
        var request = CreateRegisterRequest();

        var response = await _client.PostAsJsonAsync(_requestUri, request);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<UpdateResponse>();
        Assert.NotNull(result);
        Assert.NotEqual(Guid.Empty, result.Id);
    }

    private static RegisterRequest CreateRegisterRequest(string emailAddress = null) =>
        new()
        {
            EmailAddress = emailAddress ?? $"{Guid.NewGuid()}@example.com",
            Password = Guid.NewGuid().ToString(),
            FirstName = "Test",
            LastName = "User",
            DateOfBirth = new DateOnly(1070, 1, 1)
        };
}
