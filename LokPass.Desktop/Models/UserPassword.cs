using System;
using LokPass.Core;

namespace LokPass.Desktop.Models;

public class UserPassword(string title, string username, HashedPassword  hashedPassword)
{
    public string Title { get; set; } = title;
    public string? Username { get; set; } = username;
    public string? PasswordHash { get; set; } = Convert.ToBase64String(hashedPassword.Password);
    public string? Salt { get; set; } = Convert.ToBase64String(hashedPassword.Salt);

}