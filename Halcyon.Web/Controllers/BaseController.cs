using Halcyon.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Halcyon.Web.Controllers
{
    public class BaseController : ControllerBase
    {
        protected int CurrentUserId => int.Parse(HttpContext.User.Identity.Name);

        protected ObjectResult Generate<T>(HttpStatusCode status, T data, params string[] messages)
            where T : class
        {
            return StatusCode((int)status, new ApiResponse<T>
            {
                Data = data,
                Messages = messages.Length > 0
                    ? messages
                    : null
            });
        }

        protected ObjectResult Generate(HttpStatusCode status, params string[] messages)
            => Generate<object>(status, null, messages);
    }
}
