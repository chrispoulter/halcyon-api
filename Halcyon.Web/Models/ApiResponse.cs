using System.Collections.Generic;

namespace Halcyon.Web.Models
{
    public class ApiResponse<T> : ApiResponse
    {
        public T Data { get; set; }
    }

    public class ApiResponse
    {
        public InternalStatusCode? Code { get; set; }

        public string Message { get; set; }
    }
}