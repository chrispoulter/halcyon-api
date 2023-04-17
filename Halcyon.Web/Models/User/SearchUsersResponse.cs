namespace Halcyon.Web.Models.User
{
    public class SearchUsersResponse
    {
        public SearchUsersResponse()
        {
            Items = new List<GetUserResponse>();
        }

        public List<GetUserResponse> Items { get; set; }

        public bool HasNextPage { get; set; }

        public bool HasPreviousPage { get; set; }
    }
}