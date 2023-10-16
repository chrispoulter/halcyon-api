namespace Halcyon.Api.Features.Users.SearchUsers
{
    public class SearchUserResponse
    {
        public int Id { get; set; }

        public string EmailAddress { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public bool IsLockedOut { get; set; }

        public List<string> Roles { get; set; }
    }
}