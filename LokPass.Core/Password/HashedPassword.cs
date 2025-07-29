namespace LokPass.Core.Password;

public readonly struct HashedPassword(byte[] password, byte[] salt)
{
    public byte[] Password { get; } = password;
    public byte[] Salt { get; } = salt;
}