using System.Threading.Tasks;

namespace Halcyon.Web.Services.Hash
{
    public class HashService : IHashService
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
