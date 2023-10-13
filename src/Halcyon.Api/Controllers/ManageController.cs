using Halcyon.Api.Data;
using Halcyon.Api.Models;
using Halcyon.Api.Models.Manage;
using Halcyon.Api.Services.Hash;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;

namespace Halcyon.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    [Produces("application/json")]
    [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
    public class ManageController : BaseController
    {
        private readonly HalcyonDbContext _context;

        private readonly IHashService _hashService;

        public ManageController(HalcyonDbContext context, IHashService hashService)
        {
            _context = context;
            _hashService = hashService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(GetProfileResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetProfile()
        {
            var user = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == CurrentUserId);

            if (user is null || user.IsLockedOut)
            {
                return Problem(
                    statusCode: StatusCodes.Status404NotFound,
                    title: "User not found."
                );
            }

            var result = user.Adapt<GetProfileResponse>();

            return Ok(result);
        }

        [HttpPut]
        [ProducesResponseType(typeof(UpdateResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> UpdateProfile(UpdateProfileRequest request)
        {
            var user = await _context.Users
                 .FirstOrDefaultAsync(u => u.Id == CurrentUserId);

            if (user is null || user.IsLockedOut)
            {
                return Problem(
                    statusCode: StatusCodes.Status404NotFound,
                    title: "User not found."
                );
            }

            if (request.Version is not null && request.Version != user.Version)
            {
                return Problem(
                    statusCode: StatusCodes.Status409Conflict,
                    title: "Data has been modified since entities were loaded."
                );
            }

            if (!request.EmailAddress.Equals(user.EmailAddress, StringComparison.InvariantCultureIgnoreCase))
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
            }

            request.Adapt(user);

            await _context.SaveChangesAsync();

            return Ok(new UpdateResponse { Id = user.Id });
        }

        [HttpPut("change-password")]
        [ProducesResponseType(typeof(UpdateResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> ChangePassword(ChangePasswordRequest request)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == CurrentUserId);

            if (user is null || user.IsLockedOut)
            {
                return Problem(
                    statusCode: StatusCodes.Status404NotFound,
                    title: "User not found."
                );
            }

            if (request.Version is not null && request.Version != user.Version)
            {
                return Problem(
                     statusCode: StatusCodes.Status409Conflict,
                     title: "Data has been modified since entities were loaded."
                 );
            }

            if (user.Password is null)
            {
                return Problem(
                    statusCode: StatusCodes.Status400BadRequest,
                    title: "Incorrect password."
                );
            }

            var verified = _hashService.VerifyHash(request.CurrentPassword, user.Password);

            if (!verified)
            {
                return Problem(
                    statusCode: StatusCodes.Status400BadRequest,
                    title: "Incorrect password."
                );
            }

            user.Password = _hashService.GenerateHash(request.NewPassword);
            user.PasswordResetToken = null;

            await _context.SaveChangesAsync();

            return Ok(new UpdateResponse { Id = user.Id });
        }

        [HttpDelete]
        [ProducesResponseType(typeof(UpdateResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> DeleteProfile([FromBody(EmptyBodyBehavior = EmptyBodyBehavior.Allow)] UpdateRequest request)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == CurrentUserId);

            if (user is null || user.IsLockedOut)
            {
                return Problem(
                    statusCode: StatusCodes.Status404NotFound,
                    title: "User not found."
                );
            }

            if (request?.Version is not null && request.Version != user.Version)
            {
                return Problem(
                    statusCode: StatusCodes.Status409Conflict,
                    title: "Data has been modified since entities were loaded."
                );
            }

            _context.Users.Remove(user);

            await _context.SaveChangesAsync();

            return Ok(new UpdateResponse { Id = user.Id });
        }
    }
}
