using Halcyon.Api.Data;
using Halcyon.Api.Features;
using Halcyon.Api.Features.Account.Register;
using Halcyon.Api.Services.Hash;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Moq.EntityFrameworkCore;

namespace Halcyon.Api.Tests.Features.Account.Register;

public class RegisterControllerTests
{
    private readonly Mock<HalcyonDbContext> _mockDbContext;

    private readonly List<User> _storedUsers;

    private readonly Mock<IPasswordHasher> _mockPasswordHasher;

    private readonly RegisterController _accountController;

    public RegisterControllerTests()
    {
        _mockDbContext = new Mock<HalcyonDbContext>();
        _mockPasswordHasher = new Mock<IPasswordHasher>();
        _storedUsers = new List<User>();

        _mockDbContext.Setup(m => m.Users)
            .ReturnsDbSet(_storedUsers);

        _mockDbContext.Setup(m => m.Users.Add(It.IsAny<User>()))
            .Callback<User>(_storedUsers.Add);

        _mockDbContext.Setup(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Callback(() => _storedUsers.ForEach(user => user.Id = _storedUsers.IndexOf(user) + 1));

        _accountController = new RegisterController(
            _mockDbContext.Object,
            _mockPasswordHasher.Object);
    }

    [Fact]
    public async Task Register_WhenDuplicateEmailAddress_ShouldReturnBadRequest()
    {
        var request = new RegisterRequest
        {
            EmailAddress = "test@example.com"
        };

        _storedUsers.Add(new User
        {
            EmailAddress = request.EmailAddress
        });

        var result = await _accountController.Index(request);

        var objectResult = Assert.IsType<ObjectResult>(result);
        var response = Assert.IsType<ProblemDetails>(objectResult.Value);
        Assert.Equal(StatusCodes.Status400BadRequest, response.Status);
    }

    [Fact]
    public async Task Register_WhenRequestValid_ShouldCreateNewUser()
    {
        var request = new RegisterRequest();

        var result = await _accountController.Index(request);

        var objectResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<UpdateResponse>(objectResult.Value);
        Assert.Equal(1, response.Id);
    }
}