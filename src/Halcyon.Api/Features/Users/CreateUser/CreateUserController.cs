using Halcyon.Api.Data;
using Halcyon.Api.Services.Hash;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Halcyon.Api.Features.Users.CreateUser
{
    public class CreateUserController : BaseController
    {
        private readonly HalcyonDbContext _context;

        private readonly IPasswordHasher _passwordHasher;

        public CreateUserController(HalcyonDbContext context, IPasswordHasher passwordHasher)
        {
            _context = context;
            _passwordHasher = passwordHasher;
        }

        [HttpPost("/user")]
        [Authorize(Policy = "UserAdministratorPolicy")]
        [Tags("User")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(UpdateResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(void), StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Index(CreateUserRequest request)
        {
            var existing = await _context.Users
                .FirstOrDefaultAsync(u => u.EmailAddress == request.EmailAddress);

            if (existing is not null)
            {
                return Problem(
                    statusCode: StatusCodes.Status400BadRequest,
                    title: "User name is already taken."
                );
            }

            var user = request.Adapt<User>();
            user.Password = _passwordHasher.GenerateHash(request.Password);

            _context.Users.Add(user);

            await _context.SaveChangesAsync();

            return Ok(new UpdateResponse { Id = user.Id });
        }
    }
}
