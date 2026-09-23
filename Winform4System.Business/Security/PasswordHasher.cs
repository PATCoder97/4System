using System;
using System.Security.Cryptography;

namespace Winform4System.Business.Security
{
    public sealed class PasswordHasher
    {
        private const string Algorithm = "PBKDF2-SHA256";
        private const int DefaultIterations = 100000;
        private const int SaltSize = 16;
        private const int HashSize = 32;

        public string Hash(string password)
        {
            if (string.IsNullOrEmpty(password))
                throw new ArgumentException("Password is required.", nameof(password));

            var salt = new byte[SaltSize];
            using (RandomNumberGenerator random = RandomNumberGenerator.Create())
                random.GetBytes(salt);

            byte[] hash = Derive(password, salt, DefaultIterations, HashSize);
            return string.Join("$", Algorithm, DefaultIterations.ToString(), Convert.ToBase64String(salt), Convert.ToBase64String(hash));
        }

        public bool Verify(string password, string encodedHash)
        {
            if (string.IsNullOrEmpty(password) || string.IsNullOrWhiteSpace(encodedHash))
                return false;

            try
            {
                string[] parts = encodedHash.Split('$');
                if (parts.Length != 4 || !string.Equals(parts[0], Algorithm, StringComparison.Ordinal))
                    return false;

                if (!int.TryParse(parts[1], out int iterations) || iterations < 10000 || iterations > 1000000)
                    return false;

                byte[] salt = Convert.FromBase64String(parts[2]);
                byte[] expected = Convert.FromBase64String(parts[3]);
                if (salt.Length < 8 || expected.Length < 16)
                    return false;

                byte[] actual = Derive(password, salt, iterations, expected.Length);
                return FixedTimeEquals(actual, expected);
            }
            catch (FormatException)
            {
                return false;
            }
        }

        private static byte[] Derive(string password, byte[] salt, int iterations, int outputLength)
        {
            using (var deriveBytes = new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA256))
                return deriveBytes.GetBytes(outputLength);
        }

        private static bool FixedTimeEquals(byte[] left, byte[] right)
        {
            if (left.Length != right.Length)
                return false;

            int difference = 0;
            for (int index = 0; index < left.Length; index++)
                difference |= left[index] ^ right[index];

            return difference == 0;
        }
    }
}
