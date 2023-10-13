using Halcyon.Api.Data;

namespace Halcyon.Api.Models.User
{
    public class GetUserResponse
    {
        public int Id { get; set; }

        public string EmailAddress { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public DateTime DateOfBirth { get; set; }

        public bool IsLockedOut { get; set; }

        public List<Role> Roles { get; set; }

        public uint Version { get; set; }
    }
}