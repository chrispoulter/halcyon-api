using Halcyon.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace Halcyon.Web.Filters
{
    public class ApiExceptionFilterAttribute : ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext context)
        {
            if (context.Exception.GetType() == typeof(DbUpdateConcurrencyException))
            {
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Conflict;

                var result = new ApiResponse
                {
                    Code = "CONFLICT",
                    Message = context.Exception.Message
                };

                context.Result = new JsonResult(result);
            }
            else
            {
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                var result = new ApiResponse
                {
                    Code = "INTERNAL_SERVER_ERROR",
                    Message = context.Exception.Message
                };

                context.Result = new JsonResult(result);
            }
        }
    }
}
