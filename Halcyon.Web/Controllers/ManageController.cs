using Halcyon.Web.Data;
using Halcyon.Web.Models;
using Halcyon.Web.Models.Manage;
using Halcyon.Web.Services.Hash;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
                    Code = InternalStatusCode.USER_NOT_FOUND,
                    Message = "User not found."
                });
            }

            return Ok(new ApiResponse<GetProfileResponse>
            {
                Data =
                {
                    Id = user.Id,
                    EmailAddress = user.EmailAddress,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    DateOfBirth = user.DateOfBirth.ToUniversalTime()
                }
            });
        }

        [HttpPut]
        [ProducesResponseType(typeof(ApiResponse<UpdatedResponse>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> UpdateProfile(UpdateProfileModel model)
        {
            var user = await _context.Users
                 .FirstOrDefaultAsync(u => u.Id == CurrentUserId);

            if (user == null || user.IsLockedOut)
            {
                return NotFound(new ApiResponse
                {
                    Code = InternalStatusCode.USER_NOT_FOUND,
                    Message = "User not found."
                });
            }


            if (!model.EmailAddress.Equals(user.EmailAddress, StringComparison.InvariantCultureIgnoreCase))
            {
                var existing = await _context.Users
                    .FirstOrDefaultAsync(u => u.EmailAddress == model.EmailAddress);

                if (existing != null)
                {
                    return BadRequest(new ApiResponse
                    {
                        Code = InternalStatusCode.DUPLICATE_USER,
                        Message = $"User name \"{model.EmailAddress}\" is already taken."
                    });
                }
            }

            user.EmailAddress = model.EmailAddress;
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.DateOfBirth = model.DateOfBirth.Value.ToUniversalTime();

            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<UpdatedResponse>
            {
                Code = InternalStatusCode.PROFILE_UPDATED,
                Message = "Your profile has been updated.",
                Data = { Id = user.Id }
            });
        }

        [HttpPut("change-password")]
        [ProducesResponseType(typeof(ApiResponse<UpdatedResponse>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> ChangePassword(ChangePasswordModel model)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == CurrentUserId);

            if (user == null || user.IsLockedOut)
            {
                return NotFound(new ApiResponse
                {
                    Code = InternalStatusCode.USER_NOT_FOUND,
                    Message = "User not found."
                });
            }

            var verified = _hashService.VerifyHash(model.CurrentPassword, user.Password);
            if (!verified)
            {
                return BadRequest(new ApiResponse
                {
                    Code = InternalStatusCode.INCORRECT_PASSWORD,
                    Message = "Incorrect password."
                });
            }

            user.Password = _hashService.GenerateHash(model.NewPassword);
            user.PasswordResetToken = null;

            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<UpdatedResponse>
            {
                Code = InternalStatusCode.PASSWORD_CHANGED,
                Message = "Your password has been changed.",
                Data = { Id = user.Id }
            });
        }

        [HttpDelete]
        [ProducesResponseType(typeof(ApiResponse<UpdatedResponse>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> DeleteProfile()
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == CurrentUserId);

            if (user == null || user.IsLockedOut)
            {
                return NotFound(new ApiResponse
                {
                    Code = InternalStatusCode.USER_NOT_FOUND,
                    Message = "User not found."
                });
            }

            _context.Users.Remove(user);

            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<UpdatedResponse>
            {
                Code = InternalStatusCode.ACCOUNT_DELETED,
                Message = "Your account has been deleted.",
                Data = { Id = user.Id }
            });
        }
    }
}
