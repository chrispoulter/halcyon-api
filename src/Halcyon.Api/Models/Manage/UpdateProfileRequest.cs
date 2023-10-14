namespace Halcyon.Api.Models.Manage
{
    public class UpdateProfileRequest : UpdateRequest
    {
        public string EmailAddress { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public DateOnly? DateOfBirth { get; set; }
    }
}