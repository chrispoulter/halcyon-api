using System.ComponentModel.DataAnnotations;

namespace Halcyon.Api.Features.Manage.ChangePassword
{
    public class ChangePasswordRequest : UpdateRequest
    {
        [Display(Name = "Current Password")]
        [Required]
        public string CurrentPassword { get; set; }

        [Display(Name = "New Password")]
        [Required]
        [MinLength(8)]
        [MaxLength(50)]
        public string NewPassword { get; set; }
    }
}