namespace Halcyon.Web.Models.Manage
{
    public class UpdateProfileRequest : UpdateRequest
    {
        public string EmailAddress { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public DateTime? DateOfBirth { get; set; }
    }
}