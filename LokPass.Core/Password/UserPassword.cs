namespace LokPass.Core.Password;

public class UserPassword(
    string title,
    string username,
    HashedPassword hashedPassword
) : IEquatable<UserPassword>
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Title { get; private set; } = title;
    public string Username { get; private set; } = username;
    public string PasswordHash { get; private set; } = Convert.ToBase64String(hashedPassword.Password);
    public string Salt { get; private set; } = Convert.ToBase64String(hashedPassword.Salt);
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public bool Equals(UserPassword? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;

        return Title == other.Title &&
               Username == other.Username &&
               PasswordHash == other.PasswordHash &&
               Salt == other.Salt;
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

    /// <summary>
    ///     Updating the password with the related salt
    /// </summary>
    /// <param name="hashedPassword"></param>
    public void UpdatePassword(HashedPassword hashedPassword)
    {
        var newPasswordHash = Convert.ToBase64String(hashedPassword.Password);
        var newSalt = Convert.ToBase64String(hashedPassword.Salt);

        if (PasswordHash == newPasswordHash && Salt == newSalt) return;

        PasswordHash = newPasswordHash;
        Salt = newSalt;
        UpdatedAt = DateTime.UtcNow;
    }

    public HashedPassword? GetHashedPassword()
    {
        if (string.IsNullOrEmpty(PasswordHash) || string.IsNullOrEmpty(Salt))
            return null;

        var passwordBytes = Convert.FromBase64String(PasswordHash);
        var saltBytes = Convert.FromBase64String(Salt);

        return new HashedPassword(passwordBytes, saltBytes);
    }

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((UserPassword)obj);
    }

    public override int GetHashCode()
    {
        // TODO: non-readonly properties referenced -> How to solve this?
        return HashCode.Combine(Title, Username, PasswordHash, Salt);
    }
}