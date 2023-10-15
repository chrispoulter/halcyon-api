using System.ComponentModel.DataAnnotations;

namespace Halcyon.Api.Features.Account.ResetPassword
{
    public class ResetPasswordRequest
    {
        [Display(Name = "Token")]
        [Required]
        public Guid Token { get; set; }

        [Display(Name = "Email Address")]
        [Required]
        [EmailAddress]
        public string EmailAddress { get; set; }

        [Display(Name = "New Password")]
        [Required]
        [MinLength(8)]
        [MaxLength(50)]
        public string NewPassword { get; set; }
    }
}