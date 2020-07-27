using Halcyon.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace Halcyon.Web.Controllers
{
    public class BaseController : ControllerBase
    {
        public int CurrentUserId => int.Parse(HttpContext.User.Identity.Name);

        public OkObjectResult Ok<TResult>(TResult data, params string[] messages)
            where TResult : class
        {
            return base.Ok(new ApiResult
            {
                Data = data,
                Messages = messages.Length > 0 
                    ? messages 
                    : null
            });
        }

        public OkObjectResult Ok(params string[] messages) 
        {
            return base.Ok(new ApiResult
            {
                Messages = messages.Length > 0
                    ? messages
                    : null
            });
        }

        public NotFoundObjectResult NotFound(params string[] messages)
        {
            return base.NotFound(new ApiResult
            {
                Messages = messages.Length > 0
                    ? messages
                    : null
            });
        }

        public BadRequestObjectResult BadRequest(params string[] messages)
        {
            return base.BadRequest(new ApiResult
            {
                Messages = messages.Length > 0
                    ? messages
                    : null
            });
        }
    }
}
