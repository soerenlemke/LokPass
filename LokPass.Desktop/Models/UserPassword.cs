namespace LokPass.Desktop.Models;

public class UserPassword(string title, string username, string passwordHash, string salt)
{
    public string Title { get; set; } = title;
    public string? Username { get; set; } = username;
    public string? PasswordHash { get; set; } = passwordHash;
    public string? Salt { get; set; } = salt;
}