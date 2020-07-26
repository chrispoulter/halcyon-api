namespace Halcyon.Web.Services.Password
{
    public class PasswordService : IPasswordService
    {
        public string GenerateHash(string str) 
            => BCrypt.Net.BCrypt.HashPassword(str, 10);

        public bool VerifyHash(string str, string hash) 
            => BCrypt.Net.BCrypt.Verify(str, hash);
    }
}
