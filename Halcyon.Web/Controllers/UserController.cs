using Halcyon.Web.Data;
using Halcyon.Web.Models.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Halcyon.Web.Controllers
{
    [Route("[controller]")]
    [Authorize(Roles = SystemRoles.UserAdministrator)]
    public class UserController : BaseController
    {
        private readonly ILogger<UserController> _logger;

        public UserController(ILogger<UserController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IActionResult ListUsers()
        {
            var result = new ListUsersResult();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public IActionResult GetUser(int id)
        {
            var result = new GetUserResult();
            return Ok(result);
        }

        [HttpPost]
        public IActionResult CreateUser(CreateUserModel model)
        {
            return Ok();
        }

        [HttpPut("{id}")]
        public IActionResult UpdateUser(int id, UpdateUserModel model)
        {
            return Ok();
        }

        [HttpPut("{id}/lock")]
        public IActionResult LockUser(int id)
        {
            return Ok();
        }

        [HttpPut("{id}/unlock")]
        public IActionResult UnlockUser(int id)
        {
            return Ok();
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteUser(int id)
        {
            return Ok();
        }
    }
}
