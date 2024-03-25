using System.Net;
using System.Net.Http.Json;
using Halcyon.Api.Features.Manage.GetProfile;

namespace Halcyon.Api.Tests.Features.Manage.GetProfile;

public class GetProfileEndpointTests : IClassFixture<TestWebApplicationFactory>
{
    private const string RequestUri = "/manage";

    private readonly TestWebApplicationFactory factory;

    public GetProfileEndpointTests(TestWebApplicationFactory factory)
    {
        this.factory = factory;
    }

    [Fact]
    public async Task GetProfile_ShouldReturnUnauthorized_WhenNotAuthorized()
    {
        var client = factory.CreateClient();
        var response = await client.GetAsync(RequestUri);
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetProfile_ShouldReturnProfile_WhenAuthorized()
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
