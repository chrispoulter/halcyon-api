using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Halcyon.Web.Filters.Validation;

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
        [RedirectUrl]
        public string SiteUrl { get; set; }
    }
}