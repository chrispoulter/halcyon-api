using Halcyon.Api.Filters;
using System.ComponentModel.DataAnnotations;

namespace Halcyon.Api.Features.Manage.UpdateProfile
{
    public class UpdateProfileRequest : UpdateRequest
    {
        [Display(Name = "Email Address")]
        [Required]
        [EmailAddress]
        [MaxLength(255)]
        public string EmailAddress { get; set; }

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