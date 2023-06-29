using Halcyon.Web.Data;
using Halcyon.Web.Models;
using Halcyon.Web.Models.Manage;
using Halcyon.Web.Services.Hash;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace Halcyon.Web.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.Unauthorized)]
    [Route("[controller]")]
    [Authorize]
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
        [ProducesResponseType(typeof(ApiResponse<GetProfileResponse>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetProfile()
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == CurrentUserId);

            if (user == null || user.IsLockedOut)
            {
                return NotFound(new ApiResponse
                {
                    Code = "USER_NOT_FOUND",
                    Message = "User not found."
                });
            }

            return Ok(new ApiResponse<GetProfileResponse>
            {
                Data = new GetProfileResponse
                {
                    Id = user.Id,
                    EmailAddress = user.EmailAddress,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    DateOfBirth = user.DateOfBirth.ToUniversalTime(),
                    Version = user.Version
                }
            });
        }

        [HttpPut]
        [ProducesResponseType(typeof(ApiResponse<UpdatedResponse>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.Conflict)]
        public async Task<IActionResult> UpdateProfile(UpdateProfileRequest request)
        {
            var user = await _context.Users
                 .FirstOrDefaultAsync(u => u.Id == CurrentUserId);

            if (user == null || user.IsLockedOut)
            {
                return NotFound(new ApiResponse
                {
                    Code = "USER_NOT_FOUND",
                    Message = "User not found."
                });
            }

            if (request.Version != null && request.Version != user.Version)
            {
                return Conflict(new ApiResponse
                {
                    Code = "CONFLICT",
                    Message = "Data has been modified or deleted since entities were loaded."
                });
            }

            if (!request.EmailAddress.Equals(user.EmailAddress, StringComparison.InvariantCultureIgnoreCase))
            {
                var existing = await _context.Users
                    .FirstOrDefaultAsync(u => u.EmailAddress == request.EmailAddress);

                if (existing != null)
                {
                    return BadRequest(new ApiResponse
                    {
                        Code = "DUPLICATE_USER",
                        Message = $"User name \"{request.EmailAddress}\" is already taken."
                    });
                }
            }

            user.EmailAddress = request.EmailAddress;
            user.FirstName = request.FirstName;
            user.LastName = request.LastName;
            user.DateOfBirth = request.DateOfBirth.Value.ToUniversalTime();
            user.Version = Guid.NewGuid();

            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<UpdatedResponse>
            {
                Code = "PROFILE_UPDATED",
                Message = "Your profile has been updated.",
                Data = new UpdatedResponse { Id = user.Id }
            });
        }

        [HttpPut("change-password")]
        [ProducesResponseType(typeof(ApiResponse<UpdatedResponse>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.Conflict)]
        public async Task<IActionResult> ChangePassword(ChangePasswordRequest request)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == CurrentUserId);

            if (user == null || user.IsLockedOut)
            {
                return NotFound(new ApiResponse
                {
                    Code = "USER_NOT_FOUND",
                    Message = "User not found."
                });
            }

            if (request.Version != null && request.Version != user.Version)
            {
                return Conflict(new ApiResponse
                {
                    Code = "CONFLICT",
                    Message = "Data has been modified or deleted since entities were loaded."
                });
            }

            if (user.Password == null)
            {
                return BadRequest(new ApiResponse
                {
                    Code = "INCORRECT_PASSWORD",
                    Message = "Incorrect password."
                });
            }

            var verified = _hashService.VerifyHash(request.CurrentPassword, user.Password);

            if (!verified)
            {
                return BadRequest(new ApiResponse
                {
                    Code = "INCORRECT_PASSWORD",
                    Message = "Incorrect password."
                });
            }

            user.Password = _hashService.GenerateHash(request.NewPassword);
            user.PasswordResetToken = null;

            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<UpdatedResponse>
            {
                Code = "PASSWORD_CHANGED",
                Message = "Your password has been changed.",
                Data = new UpdatedResponse { Id = user.Id }
            });
        }

        [HttpDelete]
        [ProducesResponseType(typeof(ApiResponse<UpdatedResponse>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.Conflict)]
        public async Task<IActionResult> DeleteProfile([FromBody(EmptyBodyBehavior = EmptyBodyBehavior.Allow)] UpdateRequest request)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == CurrentUserId);

            if (user == null || user.IsLockedOut)
            {
                return NotFound(new ApiResponse
                {
                    Code = "USER_NOT_FOUND",
                    Message = "User not found."
                });
            }

            if (request?.Version != null && request.Version != user.Version)
            {
                return Conflict(new ApiResponse
                {
                    Code = "CONFLICT",
                    Message = "Data has been modified or deleted since entities were loaded."
                });
            }

            _context.Users.Remove(user);

            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<UpdatedResponse>
            {
                Code = "ACCOUNT_DELETED",
                Message = "Your account has been deleted.",
                Data = new UpdatedResponse { Id = user.Id }
            });
        }
    }
}
