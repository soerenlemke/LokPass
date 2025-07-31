namespace LokPass.Core.Password.Crypto;

public class UserConfiguration
{
    public byte[] MasterKeySalt { get; set; } = [];
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public static UserConfiguration CreateNew(ICryptoService cryptoService)
    {
        return new UserConfiguration
        {
            MasterKeySalt = cryptoService.GenerateUserSalt()
        };
    }

    public static UserConfiguration CreateWithSalt(byte[] masterKeySalt)
    {
        return new UserConfiguration
        {
            MasterKeySalt = masterKeySalt
        };
    }
}