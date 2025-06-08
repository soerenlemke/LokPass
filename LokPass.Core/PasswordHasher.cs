using System.Security.Cryptography;

namespace LokPass.Core;

public class PasswordHasher(int saltSize = 16, int keySize = 32, int iterations = 100_000)
{
    public string HashPassword(string password)
    {
        var salt = new byte[saltSize];
        RandomNumberGenerator.Fill(salt);

        var hash = Rfc2898DeriveBytes.Pbkdf2(
            password,
            salt,
            iterations,
            HashAlgorithmName.SHA256,
            keySize);

        return Convert.ToBase64String(salt) + ":" + Convert.ToBase64String(hash);
    }

    public bool VerifyHashedPassword(string password, string storedHashedPassword)
    {
        var parts = storedHashedPassword.Split(':');
        if (parts.Length != 2)
            return false;

        var salt = Convert.FromBase64String(parts[0]);
        var hash = Convert.FromBase64String(parts[1]);

        var hashToCheck = Rfc2898DeriveBytes.Pbkdf2(
            password,
            salt,
            iterations,
            HashAlgorithmName.SHA256,
            keySize);

        return CryptographicOperations.FixedTimeEquals(hash, hashToCheck);
    }
}