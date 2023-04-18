using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Halcyon.Web.Models.Account
{
    public class ForgotPasswordRequest
    {
        [DisplayName("Email Address")]
        [Required]
        [EmailAddress]
        public string EmailAddress { get; set; }

        [DisplayName("Site Url")]
        [Required]
        [Url]
        public string SiteUrl { get; set; }
    }
}