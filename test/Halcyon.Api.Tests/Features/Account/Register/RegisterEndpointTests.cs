using System.Net;
using System.Net.Http.Json;
using Halcyon.Api.Common;
using Halcyon.Api.Features.Account.Register;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Halcyon.Api.Tests.Features.Account.Register;

public class RegisterEndpointTests : IClassFixture<TestWebApplicationFactory<Program>>
{
    private const string RequestUri = "/account/register";

    private readonly WebApplicationFactory<Program> factory;

    public RegisterEndpointTests(TestWebApplicationFactory<Program> factory)
    {
        this.factory = factory;
    }

    [Fact]
    public async Task Register_WhenDuplicateEmailAddress_ShouldReturnBadRequest()
    {
        var user = await factory.CreateTestUserAsync();
        var request = CreateRegisterRequest(user.EmailAddress);

        var client = factory.CreateClient();
        var response = await client.PostAsJsonAsync(RequestUri, request);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        Assert.NotNull(result);
        Assert.Equal("User name is already taken.", result.Title);
    }

    [Fact]
    public async Task Register_WhenRequestValid_ShouldCreateNewUser()
    {
        var request = CreateRegisterRequest();

        var client = factory.CreateClient();
        var response = await client.PostAsJsonAsync(RequestUri, request);
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
