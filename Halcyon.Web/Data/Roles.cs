using System.ComponentModel.DataAnnotations;

namespace Halcyon.Web.Data
{
    public enum Roles
    {
        [Display(Name = "System Administrator")]
        SYSTEM_ADMINISTRATOR,

        [Display(Name = "User Administrator")]
        USER_ADMINISTRATOR
    }
}
