namespace Halcyon.Api.Models.User
{
    public class UpdateUserRequest : UpdateRequest
    {
        public string EmailAddress { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public DateOnly? DateOfBirth { get; set; }

        public List<string> Roles { get; set; }
    }
}