using System.Net;
using System.Net.Http.Json;
using Halcyon.Api.Features.Profile.GetProfile;

namespace Halcyon.Api.Tests.Features.Profile.GetProfile;

public class GetProfileEndpointTests(TestWebApplicationFactory factory) : BaseTest(factory)
{
    private const string _requestUri = "/profile";

    [Fact]
    public async Task GetProfile_ShouldReturnUnauthorized_WhenNotAuthorized()
    {
        var response = await _client.GetAsync(_requestUri);
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetProfile_ShouldReturnProfile_WhenAuthorized()
    {
        var user = await CreateTestUserAsync();
        SetTestAuth(user);

        var response = await _client.GetAsync(_requestUri);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<GetProfileResponse>();
        Assert.NotNull(result);
        Assert.Equal(user.Id, result.Id);
    }
}
