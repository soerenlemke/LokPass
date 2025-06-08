namespace LokPass.Core;

public class UserPassword(string title, string username, string passwordHash)
{
    public string Title { get; set; } = title;
    public string? Username { get; set; } = username;
    public string? PasswordHash { get; set; } = passwordHash;
}