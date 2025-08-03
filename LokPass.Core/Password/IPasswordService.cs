using LokPass.Core.Password.Crypto;

namespace LokPass.Core.Password;

public interface IPasswordService
{
    Task<IEnumerable<UserPassword>> GetAllPasswordsAsync();
    Task<UserPassword?> GetPasswordByIdAsync(Guid id);
    Task AddNewPasswordAsync(UserConfiguration userConfiguration, string title, string username, string password);

    Task EditPasswordAsync(UserConfiguration userConfiguration, Guid id, string title, string username,
        string? newPassword = null);

    Task DeletePasswordAsync(Guid id);
    Task<bool> ValidatePasswordAsync(UserConfiguration userConfiguration, Guid id, string password);
}