using System.Net;
using System.Net.Http.Json;
using Halcyon.Api.Features.Manage.GetProfile;

namespace Halcyon.Api.Tests.Features.Manage.GetProfile;

public class GetProfileEndpointTests : BaseTest
{
    private const string _requestUri = "/manage";

    public GetProfileEndpointTests(TestWebApplicationFactory factory)
        : base(factory) { }

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
