using Halcyon.Api.Common;
using Halcyon.Api.Data;
using Halcyon.Api.Features.Account.Register;
using Halcyon.Api.Services.Hash;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Moq;
using Moq.EntityFrameworkCore;

namespace Halcyon.Api.Tests.Features.Account.Register;

public class RegisterEndpointTests
{
    private readonly Mock<HalcyonDbContext> mockDbContext;

    private readonly List<User> storedUsers;

    private readonly Mock<IPasswordHasher> mockPasswordHasher;

    public RegisterEndpointTests()
    {
        mockDbContext = new Mock<HalcyonDbContext>();
        mockPasswordHasher = new Mock<IPasswordHasher>();
        storedUsers = [];

        mockDbContext.Setup(m => m.Users)
            .ReturnsDbSet(storedUsers);

        mockDbContext.Setup(m => m.Users.Add(It.IsAny<User>()))
            .Callback<User>(storedUsers.Add);

        mockDbContext.Setup(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Callback(() => storedUsers.ForEach(user => user.Id = storedUsers.IndexOf(user) + 1));
    }

    [Fact]
    public async Task Register_WhenDuplicateEmailAddress_ShouldReturnBadRequest()
    {
        var request = new RegisterRequest
        {
            EmailAddress = "test@example.com"
        };

        storedUsers.Add(new User
        {
            EmailAddress = request.EmailAddress
        });

        var result = await RegisterEndpoint.HandleAsync(
            request,
            mockDbContext.Object,
            mockPasswordHasher.Object
        );

        var response = Assert.IsType<ProblemHttpResult>(result);
        Assert.Equal(StatusCodes.Status400BadRequest, response.StatusCode);
        Assert.Equal("User name is already taken.", response.ProblemDetails.Title);
    }

    [Fact]
    public async Task Register_WhenRequestValid_ShouldCreateNewUser()
    {
        var request = new RegisterRequest();

        var result = await RegisterEndpoint.HandleAsync(
            request,
            mockDbContext.Object,
            mockPasswordHasher.Object
        );

        var response = Assert.IsType<Ok<UpdateResponse>>(result);
        Assert.NotNull(response.Value);
        Assert.Equal(1, response.Value.Id);
    }
}