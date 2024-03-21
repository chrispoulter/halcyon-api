using FluentAssertions;
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
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var result = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        result.Should().NotBeNull();
        result.Title.Should().Be("User name is already taken.");
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
        result.Should().NotBeNull();
        result.Id.Should().BeGreaterThan(0);
    }
}
