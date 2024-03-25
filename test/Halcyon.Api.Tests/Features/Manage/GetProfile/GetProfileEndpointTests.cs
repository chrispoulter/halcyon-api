using System.Net;
using System.Net.Http.Json;
using Halcyon.Api.Features.Manage.GetProfile;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Halcyon.Api.Tests.Features.Manage.GetProfile;

public class GetProfileEndpointTests : IClassFixture<TestWebApplicationFactory<Program>>
{
    private const string RequestUri = "/manage";

    private readonly WebApplicationFactory<Program> factory;

    public GetProfileEndpointTests(TestWebApplicationFactory<Program> factory)
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
        var user = await factory.CreateTestUserAsync();

        var client = factory.CreateClient();
        client.SetTestAuth(user);

        var response = await client.GetAsync(RequestUri);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<GetProfileResponse>();
        Assert.NotNull(result);
        Assert.Equal(user.Id, result.Id);
    }
}
