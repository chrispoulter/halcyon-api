using Halcyon.Api.Common;
using Halcyon.Api.Data;
using Halcyon.Api.Features.Account.Register;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;

namespace Halcyon.Api.Tests.Features.Account.Register;

public class RegisterEndpointTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> factory;
    private readonly HttpClient client;

    public RegisterEndpointTests(CustomWebApplicationFactory<Program> factory)
    {
        this.factory = factory;
        this.client = factory.CreateClient();
    }

    [Fact]
    public async Task Register_WhenDuplicateEmailAddress_ShouldReturnBadRequest()
    {
        using var scope = factory.Services.CreateScope();
        using var dbContext = scope.ServiceProvider.GetRequiredService<HalcyonDbContext>();

        var user = new User
        {
            EmailAddress = $"{Guid.NewGuid()}@example.com",
            Password = "password",
            FirstName = "Test",
            LastName = "User",
            DateOfBirth = new DateOnly(2000, 1, 1)
        };

        dbContext.Users.Add(user);

        await dbContext.SaveChangesAsync();

        var request = new RegisterRequest
        {
            EmailAddress = user.EmailAddress,
            Password = user.Password,
            FirstName = user.FirstName,
            LastName = user.LastName,
            DateOfBirth = user.DateOfBirth
        };

        var response = await client.PostAsJsonAsync("/account/register", request);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        Assert.NotNull(result);
        Assert.Equal("User name is already taken.", result.Title);
    }

    [Fact]
    public async Task Register_WhenRequestValid_ShouldCreateNewUser()
    {
        var request = new RegisterRequest
        {
            EmailAddress = $"{Guid.NewGuid()}@example.com",
            Password = "password",
            FirstName = "Test",
            LastName = "User",
            DateOfBirth = new DateOnly(2000, 1, 1)
        };

        var response = await client.PostAsJsonAsync("/account/register", request);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<UpdateResponse>();
        Assert.NotNull(result);
        Assert.NotEqual(0, result.Id);
    }
}
