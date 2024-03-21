using AutoFixture;
using FluentAssertions;
using Halcyon.Api.Common;
using Halcyon.Api.Data;
using Halcyon.Api.Features.Account.Register;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;
using System.Net.Mail;

namespace Halcyon.Api.Tests.Features.Account.Register;

public class RegisterEndpointTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> factory;
    private readonly HttpClient client;
    private readonly Fixture fixture;

    public RegisterEndpointTests(CustomWebApplicationFactory<Program> factory)
    {
        this.factory = factory;
        this.client = factory.CreateClient();
        this.fixture = new Fixture();
    }

    [Fact]
    public async Task Register_WhenDuplicateEmailAddress_ShouldReturnBadRequest()
    {
        using var scope = factory.Services.CreateScope();
        using var dbContext = scope.ServiceProvider.GetRequiredService<HalcyonDbContext>();

        var emailAddress = fixture.Create<MailAddress>().Address;

        var user = fixture
            .Build<User>()
            .With(p => p.EmailAddress, emailAddress)
            .With(p => p.DateOfBirth, new DateOnly(1970, 1, 1))
            .Create();

        dbContext.Users.Add(user);

        await dbContext.SaveChangesAsync();

        var request = fixture
            .Build<RegisterRequest>()
            .With(p => p.EmailAddress, user.EmailAddress)
            .With(p => p.DateOfBirth, user.DateOfBirth)
            .Create();

        var response = await client.PostAsJsonAsync("/account/register", request);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var result = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        result.Should().NotBeNull();
        result!.Title.Should().Be("User name is already taken.");
    }

    [Fact]
    public async Task Register_WhenRequestValid_ShouldCreateNewUser()
    {
        var emailAddress = fixture.Create<MailAddress>().Address;

        var request = fixture
            .Build<RegisterRequest>()
            .With(p => p.EmailAddress, emailAddress)
            .With(p => p.DateOfBirth, new DateOnly(1970, 1, 1))
            .Create();

        var response = await client.PostAsJsonAsync("/account/register", request);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<UpdateResponse>();
        result.Should().NotBeNull();
        result!.Id.Should().BeGreaterThan(0);
    }
}
