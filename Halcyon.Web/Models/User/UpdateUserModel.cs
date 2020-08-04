using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Halcyon.Web.Models.User
{
    public class UpdateUserModel
    {
        public UpdateUserModel()
        {
            Roles = new List<string>();
        }

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

        public List<string> Roles { get; set; }
    }
}