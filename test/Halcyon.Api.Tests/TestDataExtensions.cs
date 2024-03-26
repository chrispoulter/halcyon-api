using Halcyon.Api.Data;
using MassTransit;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace Halcyon.Api.Tests;

public static class TestDataExtensions
{
    public static IConsumerTestHarness<T> GetConsumerTestHarness<T>(
        this TestWebApplicationFactory factory
    )
        where T : class, IConsumer
    {
        var testHarness = factory.Services.GetTestHarness();
        return testHarness.GetConsumerHarness<T>();
    }

    public static async Task<User> CreateTestUserAsync(this TestWebApplicationFactory factory)
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

    public static void SetTestAuth(this HttpClient client, User user) =>
        client.DefaultRequestHeaders.Add(TestAuthenticationHandler.UserId, user.Id.ToString());
}
