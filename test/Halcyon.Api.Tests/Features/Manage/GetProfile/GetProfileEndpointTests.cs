using Halcyon.Api.Data;
using Halcyon.Api.Features.Manage.GetProfile;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;

namespace Halcyon.Api.Tests.Features.Manage.GetProfile;

public class GetProfileEndpointTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private const string RequestUri = "/manage";

    private readonly WebApplicationFactory<Program> factory;

    public GetProfileEndpointTests(CustomWebApplicationFactory<Program> factory)
    {
        this.factory = factory;
    }

    [Fact]
    public async Task GetProfile_WhenNotAuthorized_ShouldReturnUnauthorized()
    {
        var client = factory.CreateClient();
        var response = await client.GetAsync(RequestUri);
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetProfile_WhenAuthorized_ShouldReturnProfile()
    {
        var user = await CreateTestUser();

        var client = factory.CreateClient();
        client.DefaultRequestHeaders.Add(TestAuthenticationHandler.UserId, user.Id.ToString());

        var response = await client.GetAsync(RequestUri);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<GetProfileResponse>();
        Assert.NotNull(result);
        Assert.Equal(user.Id, result.Id);
    }

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
