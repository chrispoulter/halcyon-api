namespace Halcyon.Api.Models.Manage
{
    public class GetProfileResponse
    {
        public int Id { get; set; }

        public string EmailAddress { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public DateOnly DateOfBirth { get; set; }

        public uint Version { get; set; }
    }
}