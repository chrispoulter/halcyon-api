using Halcyon.Api.Data;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace Halcyon.Api.Tests;

public static class TestDataExtensions
{
    public static async Task<User> CreateTestUserAsync(this WebApplicationFactory<Program> factory)
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
