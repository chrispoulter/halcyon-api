using Halcyon.Web.Data;
using Halcyon.Web.Models.Manage;
using Halcyon.Web.Services.Password;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace Halcyon.Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class ManageController : BaseController
    {
        private readonly HalcyonDbContext _context;

        private readonly IPasswordService _hashService;

        public ManageController(HalcyonDbContext context, IPasswordService hashService)
        {
            _context = context;
            _hashService = hashService;
        }

        [HttpGet]
        public async Task<IActionResult> GetProfile()
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == CurrentUserId);

            if (user == null || user.IsLockedOut)
            {
                return NotFound("User not found.");
            }

            var result = new GetProfileResult
            {
                Id = user.Id,
                EmailAddress = user.EmailAddress,
                FirstName = user.FirstName,
                LastName = user.LastName,
                DateOfBirth = user.DateOfBirth
            };

            return Ok(result);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateProfile(UpdateProfileModel model)
        {
            var user = await _context.Users
                 .FirstOrDefaultAsync(u => u.Id == CurrentUserId);

            if (user == null || user.IsLockedOut)
            {
                return NotFound("User not found.");
            }


            if (!model.EmailAddress.Equals(user.EmailAddress, StringComparison.InvariantCultureIgnoreCase))
            {
                var existing = await _context.Users
                    .FirstOrDefaultAsync(u => u.EmailAddress == model.EmailAddress);

                if (existing != null)
                {
                    return BadRequest($"User name \"{model.EmailAddress}\" is already taken.");
                }
            }

            user.EmailAddress = user.EmailAddress;
            user.FirstName = user.FirstName;
            user.LastName = user.LastName;
            user.DateOfBirth = user.DateOfBirth;

            await _context.SaveChangesAsync();

            return Ok("Your profile has been updated.");
        }

        [HttpPut("changepassword")]
        public async Task<IActionResult> ChangePassword(ChangePasswordModel model)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == CurrentUserId);

            if (user == null || user.IsLockedOut)
            {
                return NotFound("User not found.");
            }

            var verified = _hashService.VerifyHash(model.CurrentPassword, user.Password);
            if(verified)
            {
                return BadRequest("Incorrect password.");
            }

            user.Password = _hashService.GenerateHash(model.NewPassword);
            user.PasswordResetToken = null;

            await _context.SaveChangesAsync();

            return Ok("Your password has been changed.");
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteProfile()
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == CurrentUserId);

            if (user == null || user.IsLockedOut)
            {
                return NotFound("User not found.");
            }

            _context.Users.Remove(user);

            await _context.SaveChangesAsync();

            return Ok("Your account has been deleted.");
        }
    }
}
