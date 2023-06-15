using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Halcyon.Web.Models.User
{
    public class LockUserRequest
    {
        [DisplayName("Version")]
        [Required]
        public Guid Version { get; set; }
    }
}