using Halcyon.Web.Data;
using Halcyon.Web.Models;
using Halcyon.Web.Models.Token;
using Halcyon.Web.Services.Hash;
using Halcyon.Web.Services.Jwt;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace Halcyon.Web.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [Route("[controller]")]
    public class TokenController : BaseController
    {
        private readonly HalcyonDbContext _context;

        private readonly IHashService _hashService;

        private readonly IJwtService _jwtService;

        public TokenController(
            HalcyonDbContext context,
            IHashService hashService,
            IJwtService jwtService)
        {
            _context = context;
            _hashService = hashService;
            _jwtService = jwtService;
        }

        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<string>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> CreateToken(CreateTokenRequest request)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.EmailAddress == request.EmailAddress);

            if (user == null || user.Password == null)
            {
                return BadRequest(new ApiResponse
                {
                    Code = "CREDENTIALS_INVALID",
                    Message = "The credentials provided were invalid."
                });
            }

            var verified = _hashService.VerifyHash(request.Password, user.Password);

            if (!verified)
            {
                return BadRequest(new ApiResponse
                {
                    Code = "CREDENTIALS_INVALID",
                    Message = "The credentials provided were invalid."
                });
            }

            if (user.IsLockedOut)
            {
                return BadRequest(new ApiResponse
                {
                    Code = "USER_LOCKED_OUT",
                    Message = "This account has been locked out, please try again later."
                });
            }

            return Ok(new ApiResponse<string>
            {
                Data = _jwtService.GenerateToken(user)
            });
        }
    }
}
