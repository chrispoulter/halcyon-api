using System.ComponentModel.DataAnnotations;

namespace Halcyon.Api.Features
{
    public class UpdateRequest
    {
        [Display(Name = "Version")]
        public uint? Version { get; set; }
    }
}