namespace LokPass.Core.Password.Repositories;

public interface IPasswordRepository
{
    Task<IEnumerable<UserPassword>> GetAllPasswordsAsync();
    Task<UserPassword?> GetPasswordByIdAsync(Guid id);
    Task AddPasswordAsync(UserPassword userPassword);
    Task UpdatePasswordAsync(UserPassword userPassword);
    Task DeletePasswordAsync(Guid id);
}