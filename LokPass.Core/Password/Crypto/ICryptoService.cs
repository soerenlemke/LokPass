namespace LokPass.Core.Password.Crypto;

public interface ICryptoService
{
    byte[] GenerateUserSalt();
    Task<EncryptedPassword> EncryptPasswordAsync(string password, string masterKey, byte[] userSalt);
    Task<string> DecryptPasswordAsync(EncryptedPassword encryptedPassword, string masterKey, byte[] userSalt);
}