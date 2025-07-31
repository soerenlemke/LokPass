using System.Security.Cryptography;
using System.Text;

namespace LokPass.Core.Password.Crypto;

public class CryptoService : ICryptoService
{
    private const int KeySize = 256;
    private const int IvSize = 128;
    private const int Iterations = 100000;
    private const int SaltSize = 32;

    public byte[] GenerateUserSalt()
    {
        return RandomNumberGenerator.GetBytes(SaltSize);
    }

    public async Task<string> DecryptPasswordAsync(
        EncryptedPassword encryptedPassword,
        string masterKey,
        byte[] userSalt
    )
    {
        ArgumentNullException.ThrowIfNull(encryptedPassword);
        ArgumentException.ThrowIfNullOrEmpty(masterKey);

        var key = DeriveKey(masterKey, userSalt);

        using var aes = Aes.Create();
        aes.Key = key;
        aes.IV = encryptedPassword.Iv;

        using var decryptor = aes.CreateDecryptor();
        var decryptedBytes = await Task.Run(() => decryptor.TransformFinalBlock(
            encryptedPassword.Password, 0, encryptedPassword.Password.Length));

        return Encoding.UTF8.GetString(decryptedBytes);
    }

    public async Task<EncryptedPassword> EncryptPasswordAsync(
        string password,
        string masterKey,
        byte[] userSalt
    )
    {
        ArgumentException.ThrowIfNullOrEmpty(password);
        ArgumentException.ThrowIfNullOrEmpty(masterKey);

        var key = DeriveKey(masterKey, userSalt);
        var iv = RandomNumberGenerator.GetBytes(IvSize / 8);

        using var aes = Aes.Create();
        aes.Key = key;
        aes.IV = iv;

        using var encryptor = aes.CreateEncryptor();
        var passwordBytes = Encoding.UTF8.GetBytes(password);
        var encryptedBytes =
            await Task.Run(() => encryptor.TransformFinalBlock(passwordBytes, 0, passwordBytes.Length));

        return new EncryptedPassword(encryptedBytes, iv);
    }

    private static byte[] DeriveKey(string masterKey, byte[] userSalt)
    {
        using var pbkdf2 = new Rfc2898DeriveBytes(
            Encoding.UTF8.GetBytes(masterKey),
            userSalt,
            Iterations,
            HashAlgorithmName.SHA256);

        return pbkdf2.GetBytes(KeySize / 8);
    }
}