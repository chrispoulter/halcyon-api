using Halcyon.Web.Models.Token;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Halcyon.Web.Controllers
{
    [Route("[controller]")]
    public class TokenController : BaseController
    {
        private readonly ILogger<TokenController> _logger;

        public TokenController(ILogger<TokenController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        public IActionResult CreateToken(CreateTokenModel model)
        {
            var result = new CreateTokenResult();
            return Ok(result);
        }
    }
}
