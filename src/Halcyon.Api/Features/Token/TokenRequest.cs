using System.ComponentModel.DataAnnotations;

namespace Halcyon.Api.Features.Token
{
    public class TokenRequest
    {
        [Display(Name = "Email Address")]
        [Required]
        [EmailAddress]
        public string EmailAddress { get; set; }

        [Display(Name = "Password")]
        [Required]
        public string Password { get; set; }
    }
}