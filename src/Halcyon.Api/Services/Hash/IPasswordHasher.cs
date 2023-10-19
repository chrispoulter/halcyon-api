namespace Halcyon.Api.Services.Hash;

public interface IPasswordHasher
{
    string GenerateHash(string str);

    bool VerifyHash(string str, string hash);
}
