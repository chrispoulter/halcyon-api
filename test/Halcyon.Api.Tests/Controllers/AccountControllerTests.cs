using Halcyon.Api.Controllers;
using Halcyon.Api.Data;
using Halcyon.Api.Models;
using Halcyon.Api.Models.Account;
using Halcyon.Api.Services.Email;
using Halcyon.Api.Services.Hash;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Moq.EntityFrameworkCore;
using Xunit;

namespace Halcyon.Api.Tests.Controllers
{
    public class AccountControllerTests
    {
        [Fact]
        public async Task Register_WhenRequestValid_ShouldCreateNewUser()
        {
            var newId = 1;

            var request = new RegisterRequest
            {
                EmailAddress = "test@example.com",
                Password = "password",
                FirstName = "Test",
                LastName = "User",
                DateOfBirth = new DateTime(1970, 1, 1)
            };

            var mockDbContext = new Mock<HalcyonDbContext>();

            var users = new List<User>();
            mockDbContext.Setup(m => m.Users).ReturnsDbSet(users);

            mockDbContext.Setup(m => m.Users.Add(It.IsAny<User>()))
                .Callback<User>(u => users.Add(u));

            mockDbContext.Setup(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Callback(() => users.First().Id = newId);

            var mockHashService = new Mock<IHashService>();
            var mockEmailService = new Mock<IEmailService>();

            var controller = new AccountController(
                mockDbContext.Object,
                mockHashService.Object,
                mockEmailService.Object);

            var response = await controller.Register(request) as OkObjectResult;
            Assert.NotNull(response);

            var result = response.Value as UpdateResponse;
            Assert.NotNull(result);
            Assert.Equal(newId, result.Id);
        }
    }
}