namespace LokPass.Core.Password.Crypto;

public interface ICryptoService
{
    byte[] GenerateUserSalt();
    byte[] GenerateUserMasterKey(string password);

    Task<EncryptedPassword> EncryptPasswordAsync(
        string password,
        UserConfiguration userConfiguration
    );

    Task<string> DecryptPasswordAsync(
        EncryptedPassword encryptedPassword,
        UserConfiguration userConfiguration
    );
}