namespace Halcyon.Web.Models.User
{
    public class SearchUsersResponse
    {
        public List<SearchUserResponse> Items { get; set; }

        public bool HasNextPage { get; set; }

        public bool HasPreviousPage { get; set; }
    }
}