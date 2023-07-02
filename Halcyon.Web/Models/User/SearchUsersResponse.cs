namespace Halcyon.Web.Models.User
{
    public class SearchUsersResponse
    {
        public SearchUsersResponse()
        {
            Items = new List<SearchUserResponse>();
        }

        public List<SearchUserResponse> Items { get; set; }

        public bool HasNextPage { get; set; }

        public bool HasPreviousPage { get; set; }
    }
}