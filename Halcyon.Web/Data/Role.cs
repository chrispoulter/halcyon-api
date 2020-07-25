using System;
using System.ComponentModel.DataAnnotations;

namespace Halcyon.Web.Data
{
    public class Role
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public string Name { get; set; }
    }
}
