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
        var userPassword = new UserPassword(title, username, encryptedPassword);

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

    public async Task EditPasswordAsync(UserConfiguration userConfiguration, Guid id, string title, string username,
        string? newPassword = null)
    {
        var existingPassword = await repository.GetPasswordByIdAsync(id);
        if (existingPassword == null) return;

        var hasChanges = false;

        if (existingPassword.Title != title)
        {
            existingPassword.UpdateTitle(title);
            hasChanges = true;
        }

        if (existingPassword.Username != username)
        {
            existingPassword.UpdateUsername(username);
            hasChanges = true;
        }

        if (!string.IsNullOrEmpty(newPassword))
        {
            var encryptedPassword = await cryptoService.EncryptPasswordAsync(newPassword, userConfiguration);
            existingPassword.UpdatePassword(encryptedPassword);
            hasChanges = true;
        }

        if (hasChanges) await repository.UpdatePasswordAsync(existingPassword);
    }
}