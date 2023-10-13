using System.ComponentModel;

namespace Halcyon.Api.Models
{
    public class UpdateRequest
    {
        [DisplayName("Version")]
        public uint? Version { get; set; }
    }
}