using Halcyon.Api.Controllers;
using Halcyon.Api.Data;
using Halcyon.Api.Models;
using Halcyon.Api.Models.Account;
using Halcyon.Api.Services.Hash;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Moq.EntityFrameworkCore;

namespace Halcyon.Api.Tests.Controllers
{
    public class AccountControllerTests
    {
        private readonly Mock<HalcyonDbContext> _mockDbContext;

        private readonly List<User> _storedUsers;

        private readonly Mock<IHashService> _mockHashService;

        private readonly Mock<IBus> _mockBus;

        private readonly AccountController _accountController;

        public AccountControllerTests()
        {
            _mockDbContext = new Mock<HalcyonDbContext>();
            _mockHashService = new Mock<IHashService>();
            _mockBus = new Mock<IBus>();
            _storedUsers = new List<User>();

            _mockDbContext.Setup(m => m.Users)
                .ReturnsDbSet(_storedUsers);

            _mockDbContext.Setup(m => m.Users.Add(It.IsAny<User>()))
                .Callback<User>(_storedUsers.Add);

            _mockDbContext.Setup(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Callback(() => _storedUsers.ForEach(user => user.Id = _storedUsers.IndexOf(user) + 1));

            _accountController = new AccountController(
                _mockDbContext.Object,
                _mockHashService.Object,
                _mockBus.Object);
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

            var result = await _accountController.Register(request);

            var objectResult = Assert.IsType<ObjectResult>(result);
            var response = Assert.IsType<ProblemDetails>(objectResult.Value);
            Assert.Equal(StatusCodes.Status400BadRequest, response.Status);
        }

        [Fact]
        public async Task Register_WhenRequestValid_ShouldCreateNewUser()
        {
            var request = new RegisterRequest();

            var result = await _accountController.Register(request);

            var objectResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<UpdateResponse>(objectResult.Value);
            Assert.Equal(1, response.Id);
        }
    }
}