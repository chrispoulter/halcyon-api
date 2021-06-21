using Halcyon.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;

namespace Halcyon.Web.Filters
{
    public class ApiExceptionFilterAttribute : ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext context)
        {
            context.HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            var result = new ApiResponse
            {
                Code = InternalStatusCode.INTERNAL_SERVER_ERROR,
                Message = context.Exception.Message
            };

            context.Result = new JsonResult(result);
        }
    }
}
