using System.ComponentModel.DataAnnotations;

namespace Halcyon.Web.Data
{
    public class UserRole
    {
        [Required]
        public int UserId { get; set; }

        public virtual User User { get; set; }

        [Required]
        public int RoleId { get; set; }

        public virtual Role Role { get; set; }
    }
}
