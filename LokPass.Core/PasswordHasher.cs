using System.Security.Cryptography;

namespace LokPass.Core;

public class PasswordHasher(int saltSize = 16, int keySize = 32, int iterations = 100_000)
{
    /// <summary>
    /// Returns the used salt and the hashed password as a string, divided by a colon.
    /// </summary>
    /// <param name="password">the password as a string</param>
    /// <returns></returns>
    public HashedPassword HashPassword(string password)
    {
        var salt = new byte[saltSize];
        RandomNumberGenerator.Fill(salt);

        var hash = Rfc2898DeriveBytes.Pbkdf2(
            password,
            salt,
            iterations,
            HashAlgorithmName.SHA256,
            keySize);

        return new HashedPassword(hash, salt);
    }

    /// <summary>
    /// Returns true if the given password matches the hashed password
    /// </summary>
    /// <param name="password"></param>
    /// <param name="hashedPassword"></param>
    /// <returns></returns>
    public bool IsValidPassword(HashedPassword  hashedPassword)
    {
        var hashToCheck = Rfc2898DeriveBytes.Pbkdf2(
            hashedPassword.Password,
            hashedPassword.Salt,
            iterations,
            HashAlgorithmName.SHA256,
            keySize);

        return CryptographicOperations.FixedTimeEquals(hashedPassword.Password, hashToCheck);
    }
}