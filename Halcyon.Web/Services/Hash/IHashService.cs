namespace Halcyon.Web.Services.Hash
{
    public interface IHashService
    {
        string GenerateHash(string str);

        bool VerifyHash(string str, string hash);
    }
}
