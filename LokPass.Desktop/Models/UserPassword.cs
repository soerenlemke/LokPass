namespace LokPass.Desktop.Models;

public class UserPassword(string title, string username, string password)
{
    public string Title { get; set; } = title;
    public string? Username { get; set; } = username;
    public string? Password { get; set; } = password;
}