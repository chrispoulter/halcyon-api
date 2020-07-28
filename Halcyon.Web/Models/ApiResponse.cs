using System.Collections.Generic;

namespace Halcyon.Web.Models
{
    public class ApiResponse
    {
        public IEnumerable<string> Messages { get; set; }

        public object Data { get; set; }
    }
}