namespace Halcyon.Web.Services.Password
{
    public interface IPasswordService
    {
        string GenerateHash(string str);

        bool VerifyHash(string str, string hash);
    }
}
