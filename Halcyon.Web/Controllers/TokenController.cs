using Halcyon.Web.Data;
using Halcyon.Web.Models.Token;
using Halcyon.Web.Services.Password;
using Halcyon.Web.Services.Jwt;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Halcyon.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TokenController : BaseController
    {
        private readonly HalcyonDbContext _context;

        private readonly IPasswordService _hashService;

        private readonly IJwtService _jwtService;

        public TokenController(
            HalcyonDbContext context, 
            IPasswordService hashService, 
            IJwtService jwtService)
        {
            _context = context;
            _hashService = hashService;
            _jwtService = jwtService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateToken(CreateTokenModel model)
        {
            var user = await _context.Users
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.EmailAddress == model.EmailAddress);

            if (user == null)
            {
                return BadRequest("The credentials provided were invalid.");
            }

            var verified = _hashService.VerifyHash(model.Password, user.Password);
            if (!verified)
            {
                return BadRequest("The credentials provided were invalid.");
            }

            if (user.IsLockedOut)
            {
                return BadRequest("This account has been locked out, please try again later.");
            }

            var result = _jwtService.GenerateToken(user);

            return Ok(result);
        }
    }
}
