using System.ComponentModel.DataAnnotations;

namespace Halcyon.Web.Data
{
    public enum Roles
    {
        [Display(Name = "System Administrator")]
        SystemAdministrator,

        [Display(Name = "User Administrator")]
        UserAdministrator
    }
}
