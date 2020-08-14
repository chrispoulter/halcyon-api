using Halcyon.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Halcyon.Web.Controllers
{
    public class BaseController : ControllerBase
    {
        public int CurrentUserId => int.Parse(HttpContext.User.Identity.Name);

        public ObjectResult Generate<TResult>(HttpStatusCode status, TResult data, params string[] messages)
            where TResult : class
        {
            return StatusCode((int)status, new ApiResponse
            {
                Data = data,
                Messages = messages.Length > 0 
                    ? messages 
                    : null
            });
        }

        public ObjectResult Generate(HttpStatusCode status, params string[] messages)
        {
            return StatusCode((int)status, new ApiResponse
            {
                Messages = messages.Length > 0
                    ? messages
                    : null
            });
        }
    }
}
