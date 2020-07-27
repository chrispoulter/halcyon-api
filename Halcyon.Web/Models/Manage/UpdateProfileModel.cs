using System;
using System.ComponentModel.DataAnnotations;

namespace Halcyon.Web.Models.Manage
{
    public class UpdateProfileModel
    {
        [Required]
        [EmailAddress]
        [MaxLength(255)]
        public string EmailAddress { get; set; }

        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(50)]
        public string LastName { get; set; }

        [Required]
        public DateTime DateOfBirth { get; set; }
    }
}