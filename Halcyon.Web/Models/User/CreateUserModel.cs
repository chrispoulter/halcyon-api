using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Halcyon.Web.Models.User
{
    public class CreateUserModel
    {
        public CreateUserModel()
        {
            Roles = new List<int>();
        }

        [Required]
        [EmailAddress]
        [MaxLength(255)]
        public string EmailAddress { get; set; }

        [Required]
        [MinLength(8)]
        [MaxLength(50)]
        public string Password { get; set; }

        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(50)]
        public string LastName { get; set; }

        [Required]
        public DateTime DateOfBirth { get; set; }

        public List<int> Roles { get; set; }
    }
}