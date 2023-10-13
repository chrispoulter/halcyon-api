using Halcyon.Api.Data;
using Halcyon.Api.Filters.Validation;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Halcyon.Api.Models.User
{
    public class CreateUserRequest
    {
        [DisplayName("Email Address")]
        [Required]
        [EmailAddress]
        [MaxLength(255)]
        public string EmailAddress { get; set; }

        [DisplayName("Password")]
        [Required]
        [MinLength(8)]
        [MaxLength(50)]
        public string Password { get; set; }

        [DisplayName("First Name")]
        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; }

        [DisplayName("Last Name")]
        [Required]
        [MaxLength(50)]
        public string LastName { get; set; }

        [DisplayName("Date Of Birth")]
        [Required]
        [Past]
        public DateTime? DateOfBirth { get; set; }

        public List<Role> Roles { get; set; }
    }
}