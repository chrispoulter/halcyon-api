namespace Halcyon.Api.Common.Authentication;

public interface IPasswordHasher
{
    string HashPassword(string password);

    bool VerifyPassword(string password, string hashedPassword);
}
