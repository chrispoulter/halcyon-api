using Microsoft.AspNetCore.Mvc;

namespace Halcyon.Api.Features
{
    public abstract class BaseController : ControllerBase
    {
        protected int CurrentUserId => int.Parse(HttpContext.User.Identity.Name);
    }
}
