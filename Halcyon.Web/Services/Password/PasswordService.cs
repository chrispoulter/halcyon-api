using System.Threading.Tasks;

namespace Halcyon.Web.Services.Password
{
    public class PasswordService : IPasswordService
    {
        public Task<string> GenerateHashAsync(string str)
        {
            return Task.FromResult("test");
        }

        public Task<bool> VerifyHash(string str, string hash)
        {
            return Task.FromResult(true);
        }
    }
}
