using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Halcyon.Web.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [Route("[controller]")]
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
