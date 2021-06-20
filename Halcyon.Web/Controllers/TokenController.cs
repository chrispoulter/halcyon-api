using Halcyon.Web.Data;
using Halcyon.Web.Models;
using Halcyon.Web.Models.Token;
using Halcyon.Web.Services.Hash;
using Halcyon.Web.Services.Jwt;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Threading.Tasks;

namespace Halcyon.Web.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [Route("api/[controller]")]
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
        [ProducesResponseType(typeof(ApiResponse<JwtResult>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> CreateToken(CreateTokenModel model)
        {
            var user = await _context.Users
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.EmailAddress == model.EmailAddress);

            if (user == null)
            {
                return BadRequest(new ApiResponse
                {
                    Code = InternalStatusCode.CREDENTIALS_INVALID,
                    Message = "The credentials provided were invalid."
                });
            }

            var verified = _hashService.VerifyHash(model.Password, user.Password);
            if (!verified)
            {
                return BadRequest(new ApiResponse
                {
                    Code = InternalStatusCode.CREDENTIALS_INVALID,
                    Message = "The credentials provided were invalid."
                });
            }

            if (user.IsLockedOut)
            {
                return BadRequest(new ApiResponse
                {
                    Code = InternalStatusCode.USER_LOCKED_OUT,
                    Message = "This account has been locked out, please try again later."
                });
            }

            return Ok(new ApiResponse<JwtResult>
            {
                Data = _jwtService.GenerateToken(user)
            });
        }
    }
}
