namespace Halcyon.Web.Data
{
    public class User
    {
        public int Id { get; set; }

        public string EmailAddress { get; set; }

        public string Password { get; set; }

        public Guid? PasswordResetToken { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public DateTime DateOfBirth { get; set; }

        public bool IsLockedOut { get; set; }

        public List<Role> Roles { get; set; }

        public uint Version { get; }

        public string Search { get; }
    }
}
