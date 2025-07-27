namespace LokPass.Core.Password;
public struct HashedPassword(byte[] password, byte[] salt)
{
    public byte[] Password { get; set; } = password;
    public byte[] Salt { get; set; } = salt;
}
