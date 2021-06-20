using Microsoft.AspNetCore.Mvc;

namespace Halcyon.Web.Controllers
{
    public class BaseController : ControllerBase
    {
        protected int CurrentUserId => int.Parse(HttpContext.User.Identity.Name);
    }
}
