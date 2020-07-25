using System.ComponentModel.DataAnnotations;

namespace Halcyon.Web.Data
{
    public class UserRole
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        public int RoleId { get; set; }
    }
}
