using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Halcyon.Api.Filters.Validation;

namespace Halcyon.Api.Features.Account.ForgotPassword
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