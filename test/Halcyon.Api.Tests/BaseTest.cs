using Halcyon.Api.Data;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace Halcyon.Api.Tests;

public abstract class BaseTest : IClassFixture<TestWebApplicationFactory>, IDisposable
{
    protected readonly TestWebApplicationFactory _factory;

    protected readonly HttpClient _client;

    protected readonly IServiceScope _scope;

    protected readonly ITestHarness _testHarness;

    protected readonly HalcyonDbContext _dbContext;

    public BaseTest(TestWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
        _scope = factory.Services.CreateScope();
        _testHarness = factory.Services.GetTestHarness();
        _dbContext = _scope.ServiceProvider.GetRequiredService<HalcyonDbContext>();
    }

    public async Task<User> CreateTestUserAsync()
    {
        var user = new User
        {
            EmailAddress = $"{Guid.NewGuid()}@example.com",
            FirstName = "Test",
            LastName = "User",
            DateOfBirth = new DateOnly(1070, 1, 1)
        };

        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync();

        return user;
    }

    public void SetTestAuth(User user) =>
        _client.DefaultRequestHeaders.Add(TestAuthenticationHandler.UserId, user.Id.ToString());

    public void Dispose()
    {
        _scope?.Dispose();
        _dbContext?.Dispose();
    }
}
