using System.ComponentModel;

namespace Halcyon.Web.Models
{
    public class UpdateRequest
    {
        [DisplayName("Version")]
        public uint? Version { get; set; }
    }
}