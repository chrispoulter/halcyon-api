using System.ComponentModel.DataAnnotations;

namespace Halcyon.Web.Data
{
    public class Role
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }
    }
}
