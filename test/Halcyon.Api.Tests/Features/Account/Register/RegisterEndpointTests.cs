using Halcyon.Api.Common;
using Halcyon.Api.Data;
using Halcyon.Api.Features.Account.Register;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Json;

namespace Halcyon.Api.Tests.Features.Account.Register;

public class RegisterEndpointTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private const string RequestUri = "/account/register";

    private readonly WebApplicationFactory<Program> factory;

    public RegisterEndpointTests(CustomWebApplicationFactory<Program> factory)
    {
        this.factory = factory;
    }

    [Fact]
    public async Task Register_WhenDuplicateEmailAddress_ShouldReturnBadRequest()
    {
        var user = await CreateTestUser();
        var request = CreateRegisterRequest(user.EmailAddress);

        var client = factory.CreateClient();
        var response = await client.PostAsJsonAsync(RequestUri, request);
        Assert.False(response.IsSuccessStatusCode);

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
        Assert.True(response.IsSuccessStatusCode);

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

    private async Task<User> CreateTestUser()
    {
        var user = new User
        {
            EmailAddress = $"{Guid.NewGuid()}@example.com",
            FirstName = "Test",
            LastName = "User",
            DateOfBirth = new DateOnly(1070, 1, 1)
        };

        using var scope = factory.Services.CreateScope();
        using var dbContext = scope.ServiceProvider.GetRequiredService<HalcyonDbContext>();
        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync();

        return user;
    }
}
