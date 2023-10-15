using FluentValidation;
using FluentValidation.Results;
using Halcyon.Api.Data;
using Halcyon.Api.Features;
using Halcyon.Api.Features.Account.Register;
using Halcyon.Api.Services.Hash;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Moq;
using Moq.EntityFrameworkCore;

namespace Halcyon.Api.Tests.Features.Account.Register
{
    public class RegisterEndpointTests
    {
        private readonly Mock<IValidator<RegisterRequest>> _mockValidator;

        private readonly Mock<HalcyonDbContext> _mockDbContext;

        private readonly List<User> _storedUsers;

        private readonly Mock<IHashService> _mockHashService;

        public RegisterEndpointTests()
        {
            _mockValidator = new Mock<IValidator<RegisterRequest>>();
            _mockDbContext = new Mock<HalcyonDbContext>();
            _mockHashService = new Mock<IHashService>();
            _storedUsers = new List<User>();

            _mockValidator.Setup(m => m.ValidateAsync(It.IsAny<RegisterRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            _mockDbContext.Setup(m => m.Users)
                .ReturnsDbSet(_storedUsers);

            _mockDbContext.Setup(m => m.Users.Add(It.IsAny<User>()))
                .Callback<User>(_storedUsers.Add);

            _mockDbContext.Setup(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Callback(() => _storedUsers.ForEach(user => user.Id = _storedUsers.IndexOf(user) + 1));
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

            var result = await RegisterEndpoint.HandleAsync(
                request,
                _mockValidator.Object,
                _mockDbContext.Object,
                _mockHashService.Object
            );

            var response = Assert.IsType<ProblemHttpResult>(result);
            Assert.Equal(StatusCodes.Status400BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Register_WhenRequestValid_ShouldCreateNewUser()
        {
            var request = new RegisterRequest();

            var result = await RegisterEndpoint.HandleAsync(
                request,
                _mockValidator.Object,
                _mockDbContext.Object,
                _mockHashService.Object
            );

            var response = Assert.IsType<Ok<UpdateResponse>>(result);
            Assert.NotNull(response.Value);
            Assert.Equal(1, response.Value.Id);
        }
    }
}