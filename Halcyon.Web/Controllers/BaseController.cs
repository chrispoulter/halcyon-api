using Halcyon.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace Halcyon.Web.Controllers
{
    [ApiController]
    public class BaseController : ControllerBase
    {
        public int CurrentUserId => int.Parse(HttpContext.User.Identity.Name);

        public OkObjectResult Ok<TResult>(string message, TResult data = null) 
            where TResult : class
        {
            return Ok(new ApiResult
            {
                Messages = new[] { message },
                Data = data
            });
        }

        public NotFoundObjectResult NotFound(string message)
        {
            return NotFound(new ApiResult
            {
                Messages = new[] { message }
            });
        }

        public BadRequestObjectResult BadRequest(string message)
        {
            return BadRequest(new ApiResult
            {
                Messages = new[] { message }
            });
        }
    }
}
