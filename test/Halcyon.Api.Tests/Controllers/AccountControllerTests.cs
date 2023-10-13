using Halcyon.Api.Controllers;
using Halcyon.Api.Data;
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
        public async Task Register_ShouldCreateNewUser()
        {
            var request = new RegisterRequest
            {
                EmailAddress = "test@example.com",
                Password = "password",
                FirstName = "Test",
                LastName = "User",
                DateOfBirth = new DateTime(1970, 1, 1)
            };

            var mockDbContext = new Mock<HalcyonDbContext>();
            mockDbContext.Setup(x => x.Users).ReturnsDbSet(new List<User>() { });

            var mockHashService = new Mock<IHashService>();
            var mockEmailService = new Mock<IEmailService>();

            var controller = new AccountController(
                mockDbContext.Object,
                mockHashService.Object,
                mockEmailService.Object);

            var response = await controller.Register(request) as OkObjectResult;
            Assert.NotNull(response);

            mockDbContext.Verify(m => m.Add(It.IsAny<User>()), Times.Once());
            mockDbContext.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once());

            //var result = response.Value as UpdateResponse;
            //Assert.NotNull(result);
            //Assert.Equal(42, result.Id);
        }
    }
}