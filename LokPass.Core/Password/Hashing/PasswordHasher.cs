using System.Security.Cryptography;
using System.Text;
using Konscious.Security.Cryptography;

namespace LokPass.Core.Password.Hashing;

public class PasswordHasher(int saltSize = 16, int memorySize = 65536, int iterations = 3, int degreeOfParallelism = 1)
{
    /// <summary>
    ///     Returns the used salt and the hashed password as a string, divided by a colon.
    /// </summary>
    /// <param name="password">the password as a string</param>
    /// <returns></returns>
    public async Task<HashedPassword> HashPasswordAsync(string password)
    {
        var salt = new byte[saltSize];
        RandomNumberGenerator.Fill(salt);

        using var argon2 = new Argon2id(Encoding.UTF8.GetBytes(password))
        {
            Salt = salt,
            DegreeOfParallelism = degreeOfParallelism,
            Iterations = iterations,
            MemorySize = memorySize
        };

        var hash = await argon2.GetBytesAsync(32); // 32 bytes = 256 bits

        return new HashedPassword(hash, salt);
    }

    /// <summary>
    ///     Returns true if the given password matches the hashed password
    /// </summary>
    /// <param name="password"></param>
    /// <param name="hashedPassword"></param>
    /// <returns></returns>
    public async Task<bool> IsValidPasswordAsync(string password, HashedPassword hashedPassword)
    {
        using var argon2 = new Argon2id(Encoding.UTF8.GetBytes(password))
        {
            Salt = hashedPassword.Salt,
            DegreeOfParallelism = degreeOfParallelism,
            Iterations = iterations,
            MemorySize = memorySize
        };

        var hashToCheck = await argon2.GetBytesAsync(32);

        return CryptographicOperations.FixedTimeEquals(hashedPassword.Password, hashToCheck);
    }
}