using System.Net;
using System.Net.Http.Json;
using Halcyon.Api.Features.Manage.GetProfile;

namespace Halcyon.Api.Tests.Features.Manage.GetProfile;

public class GetProfileEndpointTests : IClassFixture<TestWebApplicationFactory>
{
    private const string _requestUri = "/manage";

    private readonly TestWebApplicationFactory _factory;

    public GetProfileEndpointTests(TestWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task GetProfile_ShouldReturnUnauthorized_WhenNotAuthorized()
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync(_requestUri);
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetProfile_ShouldReturnProfile_WhenAuthorized()
    {
        var user = await _factory.CreateTestUserAsync();

        var client = _factory.CreateClient();
        client.SetTestAuth(user);

        var response = await client.GetAsync(_requestUri);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<GetProfileResponse>();
        Assert.NotNull(result);
        Assert.Equal(user.Id, result.Id);
    }
}
