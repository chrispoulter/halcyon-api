using Halcyon.Api.Data;

namespace Halcyon.Api.Models.User
{
    public class SearchUserResponse
    {
        public int Id { get; set; }

        public string EmailAddress { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public bool IsLockedOut { get; set; }

        public List<Role> Roles { get; set; }
    }
}