namespace Halcyon.Api.Features.Users.SearchUsers
{
    public class SearchUsersRequest
    {
        public string Search { get; set; }

        public UserSort Sort { get; set; } = UserSort.NAME_ASC;

        public int Page { get; set; } = 1;

        public int Size { get; set; } = 50;
    }
}
