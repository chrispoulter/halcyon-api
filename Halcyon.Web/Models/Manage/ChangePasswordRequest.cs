using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Halcyon.Web.Models.Manage
{
    public class ChangePasswordRequest : UpdateRequest
    {
        [DisplayName("Current Password")]
        [Required]
        public string CurrentPassword { get; set; }

        [DisplayName("New Password")]
        [Required]
        [MinLength(8)]
        [MaxLength(50)]
        public string NewPassword { get; set; }
    }
}