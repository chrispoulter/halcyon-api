using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Halcyon.Web.Models
{
    public class UpdateRequest
    {
        [DisplayName("Version")]
        [Required]
        public Guid Version { get; set; }
    }
}