using System.Collections.Concurrent;

namespace LokPass.Core.Password.Repositories;

public class InMemoryPasswordRepository : IPasswordRepository
{
    private readonly ConcurrentDictionary<Guid, UserPassword> _passwords = [];

    public Task<IEnumerable<UserPassword>> GetAllPasswordsAsync()
    {
        return Task.FromResult(_passwords.Values.AsEnumerable());
    }

    public Task<UserPassword?> GetPasswordByIdAsync(Guid id)
    {
        _passwords.TryGetValue(id, out var userPassword);
        return Task.FromResult(userPassword);
    }

    public Task AddPasswordAsync(UserPassword password)
    {
        return Task.FromResult(_passwords.TryAdd(password.Id, password));
    }

    public Task UpdatePasswordAsync(UserPassword password)
    {
        if (!_passwords.TryGetValue(password.Id, out var existingPassword)) return Task.CompletedTask;
        if (!HasChanges(existingPassword, password)) return Task.CompletedTask;

        password.UpdatedAt = DateTime.UtcNow;
        _passwords.TryUpdate(password.Id, password, existingPassword);

        return Task.CompletedTask;
    }

    public Task DeletePasswordAsync(Guid id)
    {
        return Task.FromResult(_passwords.TryRemove(id, out _));
    }

    private static bool HasChanges(UserPassword existing, UserPassword updated)
    {
        return existing.Title != updated.Title ||
               existing.Username != updated.Username ||
               existing.PasswordHash != updated.PasswordHash ||
               existing.Salt != updated.Salt;
    }
}