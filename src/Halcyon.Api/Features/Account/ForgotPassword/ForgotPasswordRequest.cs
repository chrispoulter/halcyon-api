using System.ComponentModel.DataAnnotations;
using Halcyon.Api.Filters;

namespace Halcyon.Api.Features.Account.ForgotPassword
{
    public class ForgotPasswordRequest
    {
        [Display(Name = "Email Address")]
        [Required]
        [EmailAddress]
        public string EmailAddress { get; set; }

        [Display(Name = "Site Url")]
        [Required]
        [RedirectUrl]
        public string SiteUrl { get; set; }
    }
}