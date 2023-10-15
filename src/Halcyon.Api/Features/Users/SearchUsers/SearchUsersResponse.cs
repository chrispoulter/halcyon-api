namespace Halcyon.Api.Features.Users.SearchUsers
{
    public class SearchUsersResponse
    {
        public List<SearchUserResponse> Items { get; set; }

        public bool HasNextPage { get; set; }

        public bool HasPreviousPage { get; set; }
    }
}