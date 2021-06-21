namespace Halcyon.Web.Models
{
    public class ApiResponse<T> : ApiResponse
        where T : new()
    {
        public T Data { get; set; } = new T();
    }

    public class ApiResponse
    {
        public InternalStatusCode? Code { get; set; }

        public string Message { get; set; }
    }
}