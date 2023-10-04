using Halcyon.Web.Data;

namespace Halcyon.Web.Models.User
{
    public class CreateUserRequest
    {
        public string EmailAddress { get; set; }

        public string Password { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public List<Role> Roles { get; set; }
    }
}