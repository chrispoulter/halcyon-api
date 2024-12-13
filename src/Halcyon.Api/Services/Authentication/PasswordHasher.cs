using System.Security.Cryptography;

namespace Halcyon.Api.Services.Authentication;

public class PasswordHasher : IPasswordHasher
{
    private readonly HashAlgorithmName Algorithm = HashAlgorithmName.SHA256;

    private const int SaltSize = 16;

    private const int KeySize = 32;

    private const int Iterations = 10000;

    public string HashPassword(string password)
    {
        using var algorithm = new Rfc2898DeriveBytes(password, SaltSize, Iterations, Algorithm);

        var key = Convert.ToBase64String(algorithm.GetBytes(KeySize));
        var salt = Convert.ToBase64String(algorithm.Salt);

        return $"{salt}.{key}";
    }

    public bool VerifyPassword(string password, string hashedPassword)
    {
        var parts = hashedPassword.Split('.', 2);
        var salt = Convert.FromBase64String(parts[0]);
        var key = Convert.FromBase64String(parts[1]);

        using var algorithm = new Rfc2898DeriveBytes(password, salt, Iterations, Algorithm);

        var keyToCheck = algorithm.GetBytes(KeySize);
        var verified = keyToCheck.SequenceEqual(key);

        return verified;
    }
}
