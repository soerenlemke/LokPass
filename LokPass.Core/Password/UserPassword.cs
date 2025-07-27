namespace LokPass.Core.Password;

public class UserPassword(string title, string username, HashedPassword hashedPassword)
{
    public string Title { get; set; } = title;
    public string? Username { get; set; } = username;
    public string? PasswordHash { get; set; } = Convert.ToBase64String(hashedPassword.Password);
}