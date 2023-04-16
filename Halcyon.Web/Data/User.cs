using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;

namespace Halcyon.Web.Data
{
    public class User
    {
        public User()
        {
            Roles = new List<Role>();
        }

        [Key]
        public int Id { get; set; }

        [Required]
        public string EmailAddress { get; set; }

        [Required]
        public string Password { get; set; }


        public string PasswordResetToken { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public DateTime DateOfBirth { get; set; }

        [Required]
        public bool IsLockedOut { get; set; }

        public List<Role> Roles { get; set; }
    }
}
