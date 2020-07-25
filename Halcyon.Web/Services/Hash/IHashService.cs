using System.Threading.Tasks;

namespace Halcyon.Web.Services.Hash
{
    public interface IHashService
    {
        Task<string> GenerateHashAsync(string str);

        Task<bool> VerifyHash(string str, string hash);
    }
}
