namespace LokPass.Core.Password.Crypto;

public class UserConfiguration(byte[] masterKey, byte[] salt)
{
    public byte[] MasterKey { get; set; } = masterKey;
    public byte[] Salt { get; set; } = salt;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}