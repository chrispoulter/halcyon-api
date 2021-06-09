using Halcyon.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Halcyon.Web.Controllers
{
    public class BaseController : ControllerBase
    {
        protected int CurrentUserId => int.Parse(HttpContext.User.Identity.Name);

        protected ObjectResult Generate<T>(
            HttpStatusCode status,
            InternalStatusCode? code,
            T data,
            params string[] messages)
            where T : class
        {
            return StatusCode((int)status, new ApiResponse<T>
            {
                Code = code,
                Data = data,
                Messages = messages.Length > 0
                    ? messages
                    : null
            });
        }

        protected ObjectResult Generate<T>(
            HttpStatusCode status,
            T data,
            params string[] messages)
            where T : class
            => Generate<object>(status, null, data, messages);

        protected ObjectResult Generate(
            HttpStatusCode status,
            InternalStatusCode code,
            params string[] messages)
            => Generate<object>(status, code, null, messages);
    }
}
