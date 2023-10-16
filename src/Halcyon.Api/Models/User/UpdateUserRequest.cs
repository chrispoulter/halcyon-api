using Halcyon.Api.Filters;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Halcyon.Api.Models.User
{
    public class UpdateUserRequest : UpdateRequest
    {
        [DisplayName("Email Address")]
        [Required]
        [EmailAddress]
        [MaxLength(255)]
        public string EmailAddress { get; set; }

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
        public DateOnly? DateOfBirth { get; set; }

        public List<string> Roles { get; set; }
    }
}