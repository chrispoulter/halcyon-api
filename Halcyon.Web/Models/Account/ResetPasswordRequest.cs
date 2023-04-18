using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Halcyon.Web.Models.Account
{
    public class ResetPasswordRequest
    {
        [DisplayName("Token")]
        [Required]
        public string Token { get; set; }

        [DisplayName("Email Address")]
        [Required]
        [EmailAddress]
        public string EmailAddress { get; set; }

        [DisplayName("New Password")]
        [Required]
        [MinLength(8)]
        [MaxLength(50)]
        public string NewPassword { get; set; }
    }
}