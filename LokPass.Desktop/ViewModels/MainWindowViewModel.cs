using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LokPass.Core;
using LokPass.Desktop.Models;
using Microsoft.Extensions.Logging;

namespace LokPass.Desktop.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private readonly ILogger<MainWindowViewModel> _logger;
    private readonly PasswordHasher _passwordHasher = new();

    [ObservableProperty] private string _newPassword = "";
    [ObservableProperty] private string _newTitle = "";
    [ObservableProperty] private string _newUsername = "";
    [ObservableProperty] private ObservableCollection<UserPassword> _userPasswords = [];

    public MainWindowViewModel()
    {
    }

    public MainWindowViewModel(ILogger<MainWindowViewModel> logger)
    {
        _logger = logger;
        _logger.LogInformation("MainWindowViewModel constructed!");
    }

    [RelayCommand]
    private void AddUserPassword()
    {
        if (string.IsNullOrWhiteSpace(NewTitle)) return;

        try
        {
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
            _logger.LogInformation("New user password added!");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to add user password");
        }
    }

    [RelayCommand]
    private void SettingsButton()
    {
        _logger.LogInformation("ClickSettingsButton");
    }

    [RelayCommand]
    private void CopyUsername()
    {
        _logger.LogInformation("ClickCopyUsername");
    }
    
    [RelayCommand]
    private void CopyPassword()
    {
        _logger.LogInformation("ClickCopyPassword");
    }
}