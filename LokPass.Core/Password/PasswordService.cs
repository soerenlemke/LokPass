using LokPass.Core.Password.Hashing;
using LokPass.Core.Password.Repositories;

namespace LokPass.Core.Password;

public class PasswordService(IPasswordRepository repository, PasswordHasher passwordHasher) : IPasswordService
{
    public async Task<IEnumerable<UserPassword>> GetAllPasswordsAsync()
    {
        return await repository.GetAllPasswordsAsync();
    }

    public async Task<UserPassword?> GetPasswordByIdAsync(Guid id)
    {
        return await repository.GetPasswordByIdAsync(id);
    }

    public async Task AddNewPasswordAsync(string title, string username, string password)
    {
        var hashedPassword = await passwordHasher.HashPasswordAsync(password);
        var userPassword = new UserPassword(title, username, hashedPassword);

        await repository.AddPasswordAsync(userPassword);
    }

    public async Task EditPasswordAsync(Guid id, string title, string username, string? newPassword = null)
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
            var hashedPassword = await passwordHasher.HashPasswordAsync(newPassword);
            existingPassword.UpdatePassword(hashedPassword);
            hasChanges = true;
        }

        if (hasChanges)
        {
            existingPassword.UpdatedAt = DateTime.UtcNow;
            await repository.UpdatePasswordAsync(existingPassword);
        }
    }

    public async Task DeletePasswordAsync(Guid id)
    {
        await repository.DeletePasswordAsync(id);
    }

    public async Task<bool> ValidatePasswordAsync(Guid id, string password)
    {
        var userPassword = await repository.GetPasswordByIdAsync(id);

        var hashedPassword = userPassword?.GetHashedPassword();
        if (hashedPassword == null) return false;

        return await passwordHasher.IsValidPasswordAsync(password, hashedPassword.Value);
    }

    /// <summary>
    ///     Validating a password against a UserPassword entry
    /// </summary>
    public async Task<bool> ValidatePasswordAsync(UserPassword userPassword, string password)
    {
        var hashedPassword = userPassword.GetHashedPassword();
        if (hashedPassword == null) return false;

        return await passwordHasher.IsValidPasswordAsync(password, hashedPassword.Value);
    }
}