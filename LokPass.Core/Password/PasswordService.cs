using LokPass.Core.Password.Repositories;

namespace LokPass.Core.Password;

public class PasswordService(IPasswordRepository passwordRepository) : IPasswordService
{
    public async Task<IEnumerable<UserPassword>> GetAllPasswordsAsync()
    {
        return await passwordRepository.GetAllPasswordsAsync();
    }

    public async Task<UserPassword?> GetPasswordByIdAsync(Guid id)
    {
        return await passwordRepository.GetPasswordByIdAsync(id);
    }

    public async Task AddPasswordAsync(UserPassword userPassword)
    {
        await passwordRepository.AddPasswordAsync(userPassword);
    }

    public async Task UpdatePasswordAsync(UserPassword userPassword)
    {
        await passwordRepository.UpdatePasswordAsync(userPassword);
    }

    public async Task DeletePasswordAsync(Guid id)
    {
        await passwordRepository.DeletePasswordAsync(id);
    }
}