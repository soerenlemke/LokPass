namespace LokPass.Core;

public class UserPassword
{
    public UserPassword()
    {
        
    }
    public UserPassword(string title, string username, string passwordHash)
    {
        Title = title;
        Username = username;
        PasswordHash = passwordHash;
    }

    public string Title { get; set; }
    public string? Username { get; set; }
    public string? PasswordHash { get; set; }
}