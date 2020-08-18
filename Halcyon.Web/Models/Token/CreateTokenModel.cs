using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Halcyon.Web.Models.Token
{
    public class CreateTokenModel
    {
        [DisplayName("Grant Type")]
        [Required]
        public GrantType? GrantType { get; set; }

        [DisplayName("Email Address")]
        [Required]
        [EmailAddress]
        public string EmailAddress { get; set; }

        [DisplayName("Password")]
        [Required]
        public string Password { get; set; }
    }
}