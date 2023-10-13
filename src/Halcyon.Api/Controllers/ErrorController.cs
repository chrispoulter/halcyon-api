using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Halcyon.Api.Controllers
{
    [Route("[controller]")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class ErrorController : ControllerBase
    {
        public IActionResult Error()
        {
            var exception = HttpContext.Features
                .Get<IExceptionHandlerFeature>()?
                .Error;

            return Problem(
                title: exception?.Message
            );
        }
    }
}
