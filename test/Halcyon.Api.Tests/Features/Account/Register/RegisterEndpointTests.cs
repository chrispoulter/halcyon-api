using System.Net;
using System.Net.Http.Json;
using Halcyon.Api.Common;
using Halcyon.Api.Features.Account.Register;
using Microsoft.AspNetCore.Mvc;

namespace Halcyon.Api.Tests.Features.Account.Register;

public class RegisterEndpointTests : IClassFixture<TestWebApplicationFactory>
{
    private const string _requestUri = "/account/register";

    private readonly TestWebApplicationFactory _factory;

    public RegisterEndpointTests(TestWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Register_ShouldReturnBadRequest_WhenDuplicateEmailAddress()
    {
        var user = await _factory.CreateTestUserAsync();
        var request = CreateRegisterRequest(user.EmailAddress);

        var client = _factory.CreateClient();
        var response = await client.PostAsJsonAsync(_requestUri, request);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        Assert.NotNull(result);
        Assert.Equal("User name is already taken.", result.Title);
    }

    [Fact]
    public async Task Register_ShouldCreateNewUser_WhenRequestValid()
    {
        var request = CreateRegisterRequest();

        var client = _factory.CreateClient();
        var response = await client.PostAsJsonAsync(_requestUri, request);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<UpdateResponse>();
        Assert.NotNull(result);
        Assert.NotEqual(0, result.Id);
    }

    private static RegisterRequest CreateRegisterRequest(string? emailAddress = null) =>
        new()
        {
            EmailAddress = emailAddress ?? $"{Guid.NewGuid()}@example.com",
            Password = "password",
            FirstName = "Test",
            LastName = "User",
            DateOfBirth = new DateOnly(1070, 1, 1)
        };
}
