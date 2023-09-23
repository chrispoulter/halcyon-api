using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Halcyon.Web.Data
{
    [Index(nameof(EmailAddress), IsUnique = true)]

    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string EmailAddress { get; set; }

        public string Password { get; set; }

        public Guid? PasswordResetToken { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public DateTime DateOfBirth { get; set; }

        [Required]
        [DefaultValue(false)]
        public bool IsLockedOut { get; set; }

        [Column(TypeName = "text[]")]
        public List<Role> Roles { get; set; }

        [ConcurrencyCheck]
        public Guid Version { get; set; }

        public string Search
        {
            get => $"{EmailAddress} {FirstName} {LastName}";

            private set { }
        }
    }
}
