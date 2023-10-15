using System.ComponentModel;

namespace Halcyon.Api.Features
{
    public class UpdateRequest
    {
        [DisplayName("Version")]
        public uint? Version { get; set; }
    }
}