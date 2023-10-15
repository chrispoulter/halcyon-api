using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Halcyon.Api.Features.Token
{
    public class TokenRequest
    {
        [DisplayName("Email Address")]
        [Required]
        [EmailAddress]
        public string EmailAddress { get; set; }

        [DisplayName("Password")]
        [Required]
        public string Password { get; set; }
    }
}