namespace LokPass.Core.Password.Crypto;

/// <summary>
///     AES-encrypted password with initialization vector
/// </summary>
/// <param name="Password">Encrypted password bytes</param>
/// <param name="Iv">AES initialization vector for decryption</param>
public sealed record EncryptedPassword(byte[] Password, byte[] Iv);