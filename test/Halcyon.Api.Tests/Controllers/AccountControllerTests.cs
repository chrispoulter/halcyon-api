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
        [Fact]
        public async Task Register_WhenDuplicateEmailAddress_ShouldReturnBadRequest()
        {
            var request = new RegisterRequest
            { 
                EmailAddress = "test@example.com" 
            };

            var mockDbContext = new Mock<HalcyonDbContext>();
            var mockHashService = new Mock<IHashService>();
            var mockBus = new Mock<IBus>();

            var storedUsers = new List<User> 
            {
                new User
                {
                    EmailAddress = request.EmailAddress
                }
            };

            mockDbContext.Setup(m => m.Users)
                .ReturnsDbSet(storedUsers);

            var controller = new AccountController(
                mockDbContext.Object,
                mockHashService.Object,
                mockBus.Object);

            var result = await controller.Register(request);

            var objectResult = Assert.IsType<ObjectResult>(result);
            var response = Assert.IsType<ProblemDetails>(objectResult.Value);

            Assert.Equal(StatusCodes.Status400BadRequest, response.Status);
        }

        [Fact]
        public async Task Register_WhenRequestValid_ShouldCreateNewUser()
        {
            var request = new RegisterRequest();

            var mockDbContext = new Mock<HalcyonDbContext>();
            var mockHashService = new Mock<IHashService>();
            var mockBus = new Mock<IBus>();

            var storedUsers = new List<User>();

            mockDbContext.Setup(m => m.Users)
                .ReturnsDbSet(storedUsers);

            mockDbContext.Setup(m => m.Users.Add(It.IsAny<User>()))
                .Callback<User>(storedUsers.Add);

            mockDbContext.Setup(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Callback(() => storedUsers.ForEach(user => user.Id = storedUsers.IndexOf(user) + 1));

            var controller = new AccountController(
                mockDbContext.Object,
                mockHashService.Object,
                mockBus.Object);

            var result = await controller.Register(request);

            var objectResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<UpdateResponse>(objectResult.Value);

            Assert.Equal(1, response.Id);
        }
    }
}