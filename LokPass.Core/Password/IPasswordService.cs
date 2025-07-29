namespace LokPass.Core.Password;

public interface IPasswordService
{
    Task<IEnumerable<UserPassword>> GetAllPasswordsAsync();
    Task<UserPassword?> GetPasswordByIdAsync(Guid id);
    Task AddNewPasswordAsync(string title, string username, string password);
    Task EditPasswordAsync(Guid id, string title, string username, string? newPassword = null);
    Task DeletePasswordAsync(Guid id);
    Task<bool> ValidatePasswordAsync(Guid id, string password);
}