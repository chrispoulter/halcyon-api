using Halcyon.Web.Models.Manage;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Halcyon.Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class ManageController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;

        public ManageController(ILogger<UserController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IActionResult GetProfile()
        {
            var result = new GetProfileResult();
            return Ok(result);
        }

        [HttpPut]
        public IActionResult UpdateProfile(UpdateProfileModel model)
        {
            return Ok();
        }

        [HttpPut("changepassword")]
        public IActionResult ChangePassword(ChangePasswordModel model)
        {
            return Ok();
        }

        [HttpDelete]
        public IActionResult DeleteProfile()
        {
            return Ok();
        }
    }
}
