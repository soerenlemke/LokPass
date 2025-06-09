using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LokPass.Core;
using LokPass.Desktop.Models;

namespace LokPass.Desktop.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private readonly PasswordHasher _passwordHasher = new();

    [ObservableProperty] private string _newPassword = "";

    [ObservableProperty] private string _newTitle = "";

    [ObservableProperty] private string _newUsername = "";

    [ObservableProperty] private ObservableCollection<UserPassword> _userPasswords = [];

    [RelayCommand]
    private void AddUserPassword()
    {
        if (string.IsNullOrWhiteSpace(NewTitle)) return;

        var hashedPassword = _passwordHasher.HashPassword(NewPassword);
        var parts = hashedPassword.Split(':');

        UserPasswords.Add(
            new UserPassword(
                NewTitle, NewUsername, parts[0], parts[1])
        );

        NewTitle = "";
        NewUsername = "";
        NewPassword = "";

        OnPropertyChanged(nameof(UserPasswords));
    }

    [RelayCommand]
    private void SettingsButton()
    {
        Console.WriteLine("ClickSettingsButton");
    }
}