namespace Halcyon.Api.Features.Manage.UpdateProfile
{
    public class UpdateProfileRequest : UpdateRequest
    {
        public string EmailAddress { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public DateOnly? DateOfBirth { get; set; }
    }
}