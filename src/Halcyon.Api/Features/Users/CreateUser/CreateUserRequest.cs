using Halcyon.Api.Data;

namespace Halcyon.Api.Features.Users.CreateUser
{
    public class CreateUserRequest
    {
        public string EmailAddress { get; set; }

        public string Password { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public DateOnly? DateOfBirth { get; set; }

        public List<Role> Roles { get; set; }
    }
}