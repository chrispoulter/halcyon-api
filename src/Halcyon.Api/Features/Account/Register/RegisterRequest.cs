using Halcyon.Api.Filters;
using System.ComponentModel.DataAnnotations;

namespace Halcyon.Api.Features.Account.Register
{
    public class RegisterRequest
    {
        [Display(Name = "Email Address")]
        [Required]
        [EmailAddress]
        [MaxLength(255)]
        public string EmailAddress { get; set; }

        [Display(Name = "Password")]
        [Required]
        [MinLength(8)]
        [MaxLength(50)]
        public string Password { get; set; }

        [Display(Name = "First Name")]
        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        [Required]
        [MaxLength(50)]
        public string LastName { get; set; }

        [Display(Name = "Date Of Birth")]
        [Required]
        [Past]
        public DateOnly? DateOfBirth { get; set; }
    }
}