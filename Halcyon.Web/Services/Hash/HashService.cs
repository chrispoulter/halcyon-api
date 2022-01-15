using System.Security.Cryptography;

namespace Halcyon.Web.Services.Hash
{
    public class HashService : IHashService
    {
        private const int SaltSize = 16;

        private const int KeySize = 32;

        private const int Iterations = 10000;

        public string GenerateHash(string str)
        {
            using var algorithm = new Rfc2898DeriveBytes(
               str,
               SaltSize,
               Iterations,
               HashAlgorithmName.SHA256);

            var key = Convert.ToBase64String(algorithm.GetBytes(KeySize));
            var salt = Convert.ToBase64String(algorithm.Salt);

            return $"{salt}.{key}";
        }

        public bool VerifyHash(string str, string hash)
        {
            var parts = hash.Split('.', 2);
            var salt = Convert.FromBase64String(parts[0]);
            var key = Convert.FromBase64String(parts[1]);

            using var algorithm = new Rfc2898DeriveBytes(
              str,
              salt,
              Iterations,
              HashAlgorithmName.SHA256);

            var keyToCheck = algorithm.GetBytes(KeySize);
            var verified = keyToCheck.SequenceEqual(key);

            return verified;
        }
    }
}
