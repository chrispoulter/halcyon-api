using System.ComponentModel.DataAnnotations;

namespace Halcyon.Web.Models.Token
{
    public class CreateTokenModel
    {
        [Required]
        public GrantType GrantType { get; set; }

        [Required]
        [EmailAddress]
        public string EmailAddress { get; set; }

        [Required]
        public string Password { get; set; }
    }
}