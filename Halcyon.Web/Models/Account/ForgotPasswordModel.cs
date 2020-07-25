using System.ComponentModel.DataAnnotations;

namespace Halcyon.Web.Models.Account
{
    public class ForgotPasswordModel
    {
        [Required]
        [EmailAddress]
        public string EmailAddress { get; set; }
    }
}