using System.ComponentModel.DataAnnotations;

namespace Halcyon.Web.Models.Token
{
    public class CreateTokenModel
    {
        [Required]
        [EmailAddress]
        public string EmailAddress { get; set; }

        [Required]
        public string Password { get; set; }
        public object Token { get; internal set; }
    }
}