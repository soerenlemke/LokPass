using System.Text.Json.Serialization;
using LokPass.Core.Password.Crypto;

namespace LokPass.Core.Password;

public class UserPassword(
    string title,
    string username,
    EncryptedPassword encryptedPassword
) : IEquatable<UserPassword>
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public string Title { get; private set; } = title;
    public string Username { get; private set; } = username;
    public EncryptedPassword EncryptedPassword { get; private set; } = encryptedPassword;
    [JsonIgnore] public string DecryptedPassword { get; set; } = "*****";
    public DateTime CreatedAt { get; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; private set; }

    public bool Equals(UserPassword? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;

        return Title == other.Title &&
               Username == other.Username &&
               EncryptedPassword == other.EncryptedPassword;
    }

    public void UpdateTitle(string title)
    {
        if (Title == title) return;

        Title = title;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateUsername(string username)
    {
        if (Username == username) return;

        Username = username;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdatePassword(EncryptedPassword encryptedPassword)
    {
        if (EncryptedPassword == encryptedPassword) return;

        EncryptedPassword = encryptedPassword;
        UpdatedAt = DateTime.UtcNow;
    }

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        return obj.GetType() == GetType() && Equals((UserPassword)obj);
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }
}