using System.Threading.Tasks;

namespace Halcyon.Web.Services.Password
{
    public interface IPasswordService
    {
        Task<string> GenerateHashAsync(string str);

        Task<bool> VerifyHash(string str, string hash);
    }
}
