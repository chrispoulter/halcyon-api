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
            switch (context.Exception)
            {
                case DbUpdateConcurrencyException:
                    context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Conflict;

                    context.Result = new JsonResult(
                          new ApiResponse
                          {
                              Code = "CONFLICT",
                              Message = "Data has been modified or deleted since entities were loaded."
                          }
                      );

                    break;

                default:
                    context.HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                    context.Result = new JsonResult(
                        new ApiResponse
                        {
                            Code = "INTERNAL_SERVER_ERROR",
                            Message = context.Exception.Message
                        }
                    );

                    break;
            }
        }
    }
}
