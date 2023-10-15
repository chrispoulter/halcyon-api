namespace Halcyon.Api.Features.Account.Register
{
    public class RegisterRequest
    {
        public string EmailAddress { get; set; }

        public string Password { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public DateOnly? DateOfBirth { get; set; }
    }
}