using LokPass.Core.Password.Crypto;
using LokPass.Core.Password.Repositories;

namespace LokPass.Core.Password;

public class PasswordService(IPasswordRepository repository, ICryptoService cryptoService) : IPasswordService
{
    public async Task<IEnumerable<UserPassword>> GetAllPasswordsAsync()
    {
        return await repository.GetAllPasswordsAsync();
    }

    public async Task<UserPassword?> GetPasswordByIdAsync(Guid id)
    {
        return await repository.GetPasswordByIdAsync(id);
    }

    public async Task DeletePasswordAsync(Guid id)
    {
        await repository.DeletePasswordAsync(id);
    }

    public async Task AddNewPasswordAsync(UserConfiguration userConfiguration, string title, string username,
        string password)
    {
        var encryptedPassword = await cryptoService.EncryptPasswordAsync(password, userConfiguration);
        var userPassword = new UserPassword(
            Guid.NewGuid(),
            title, 
            username, 
            encryptedPassword,
            DateTime.UtcNow,
            null);

        await repository.AddPasswordAsync(userPassword);
    }

    public async Task<bool> ValidatePasswordAsync(UserConfiguration userConfiguration, Guid id, string password)
    {
        var userPassword = await repository.GetPasswordByIdAsync(id);
        if (userPassword == null) return false;

        var decryptedPassword =
            await cryptoService.DecryptPasswordAsync(userPassword.EncryptedPassword, userConfiguration);

        return decryptedPassword == password;
    }

    public async Task EditPasswordAsync(
        UserConfiguration userConfiguration,
        Guid id,
        string title,
        string username,
        string? newPassword = null)
    {
        var existing = await repository.GetPasswordByIdAsync(id);
        if (existing == null) return;

        var encryptedPassword = existing.EncryptedPassword;

        if (!string.IsNullOrWhiteSpace(newPassword))
        {
            encryptedPassword = await cryptoService.EncryptPasswordAsync(
                newPassword,
                userConfiguration);
        }

        var updated = existing
            .WithTitle(title)
            .WithUsername(username)
            .WithPassword(encryptedPassword);

        await repository.UpdatePasswordAsync(updated);
    }

}