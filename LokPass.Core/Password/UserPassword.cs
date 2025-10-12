using System.Text.Json.Serialization;
using LokPass.Core.Password.Crypto;

namespace LokPass.Core.Password;

public sealed record UserPassword(
    Guid Id,
    string Title,
    string Username,
    EncryptedPassword EncryptedPassword,
    DateTime CreatedAt,
    DateTime? UpdatedAt
)
{
    public UserPassword WithTitle(string newTitle) =>
        this with { Title = newTitle, UpdatedAt = DateTime.UtcNow };

    public UserPassword WithUsername(string newUsername) =>
        this with { Username = newUsername, UpdatedAt = DateTime.UtcNow };

    public UserPassword WithPassword(EncryptedPassword newPassword) =>
        this with { EncryptedPassword = newPassword, UpdatedAt = DateTime.UtcNow };
}

public sealed record UserPasswordView(
    UserPassword Password,
    string DecryptedPassword = "*****"
)
{
    public async Task<UserPasswordView> WithDecryptedPasswordAsync(
        ICryptoService cryptoService,
        UserConfiguration userConfiguration)
    {
        var decryptedPassword =
            await cryptoService.DecryptPasswordAsync(Password.EncryptedPassword, userConfiguration);
        return this with { DecryptedPassword = decryptedPassword };
    }
}