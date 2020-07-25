using Halcyon.Web.Models.Account;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Halcyon.Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly ILogger<AccountController> _logger;

        public AccountController(ILogger<AccountController> logger)
        {
            _logger = logger;
        }

        [HttpPost("register")]
        public IActionResult Register(RegisterModel model)
        {
            return Ok();
        }

        [HttpPost("forgotpassword")]
        public IActionResult ForgotPassword(ForgotPasswordModel model)
        {
            return Ok();
        }

        [HttpPost("resetpassword")]
        public IActionResult ResetPassword(ResetPasswordModel model)
        {
            return Ok();
        }
    }
}
