using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LokPass.Core.Password;
using LokPass.Core.Password.Hashing;
using LokPass.Core.Password.Repositories;
using Microsoft.Extensions.Logging;

namespace LokPass.Desktop.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private readonly ILogger<MainWindowViewModel> _logger;
    private readonly IPasswordService _passwordService;
    [ObservableProperty] private ObservableCollection<UserPassword> _filteredPasswords = [];

    [ObservableProperty] private string _newPassword = "";
    [ObservableProperty] private string _newTitle = "";
    [ObservableProperty] private string _newUsername = "";
    [ObservableProperty] private string _searchText = "";
    [ObservableProperty] private ObservableCollection<UserPassword> _userPasswords = [];

    // Parameterless constructor for designer
    public MainWindowViewModel()
    {
        // For designer - use InMemory Repository
        var repository = new InMemoryPasswordRepository();
        var hasher = new PasswordHasher();
        _passwordService = new PasswordService(repository, hasher);
        _logger = null!; // Designer doesn't need logger

        // Dummy data for designer
        _ = LoadPasswordsAsync();

        FilteredPasswords = [];
    }

    public MainWindowViewModel(ILogger<MainWindowViewModel> logger, IPasswordService passwordService)
    {
        _logger = logger;
        _passwordService = passwordService;
        _logger.LogInformation("MainWindowViewModel constructed!");

        _ = LoadPasswordsAsync();
    }

    partial void OnSearchTextChanged(string value)
    {
        FilterPasswords();
    }

    private void FilterPasswords()
    {
        if (string.IsNullOrEmpty(SearchText))
        {
            FilteredPasswords = new ObservableCollection<UserPassword>(UserPasswords);
            return;
        }

        var filtered = UserPasswords.Where(p =>
            p.Title.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
            p.Username.Contains(SearchText, StringComparison.OrdinalIgnoreCase)).ToList();

        FilteredPasswords = new ObservableCollection<UserPassword>(filtered);
    }

    private async Task LoadPasswordsAsync()
    {
        try
        {
            var passwords = await _passwordService.GetAllPasswordsAsync();

            UserPasswords.Clear();
            foreach (var password in passwords) UserPasswords.Add(password);

            FilterPasswords();

            _logger.LogInformation("Loaded {userPasswordsCount} passwords", UserPasswords.Count);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to load passwords");
        }
    }

    [RelayCommand]
    private async Task AddUserPassword()
    {
        if (string.IsNullOrWhiteSpace(NewTitle)) return;
        if (string.IsNullOrWhiteSpace(NewUsername)) return;
        if (string.IsNullOrWhiteSpace(NewPassword)) return;

        try
        {
            await _passwordService.AddNewPasswordAsync(NewTitle, NewUsername, NewPassword);

            await RefreshPasswordsAsync();

            NewTitle = "";
            NewUsername = "";
            NewPassword = "";

            _logger.LogInformation("New user password added!");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to add user password");
        }
    }

    private async Task RefreshPasswordsAsync()
    {
        try
        {
            var passwords = await _passwordService.GetAllPasswordsAsync();

            UpdateObservableCollection(passwords);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to refresh passwords");
        }
    }

    /// <summary>
    ///     Intelligently update the ObservableCollection without reloading everything
    /// </summary>
    private void UpdateObservableCollection(IEnumerable<UserPassword> newPasswords)
    {
        var newPasswordsList = newPasswords.ToList();

        for (var i = UserPasswords.Count - 1; i >= 0; i--)
        {
            var existing = UserPasswords[i];
            if (newPasswordsList.All(p => p.Id != existing.Id)) UserPasswords.RemoveAt(i);
        }

        foreach (var newPassword in newPasswordsList)
        {
            var userPasswordsList = UserPasswords.ToList();
            var existingIndex = userPasswordsList.FindIndex(p => p.Id == newPassword.Id);

            if (existingIndex >= 0)
            {
                if (!UserPasswords[existingIndex].Equals(newPassword)) UserPasswords[existingIndex] = newPassword;
            }
            else
            {
                UserPasswords.Add(newPassword);
            }
        }

        FilterPasswords();
    }

    [RelayCommand]
    private void SettingsButton()
    {
        _logger.LogInformation("ClickSettingsButton");
    }

    // commands per password entry

    [RelayCommand]
    private void ShowPassword(UserPassword userPassword)
    {
        try
        {
            // show decrypted password
            _logger.LogInformation("Show password requested for: {userPasswordTitle}", userPassword.Title);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to show password");
        }
    }

    [RelayCommand]
    private void CopyUsername(UserPassword userPassword)
    {
        try
        {
            // TODO: use clipboard
            // System.Windows.Clipboard.SetText(userPassword.Username);
            _logger.LogInformation("Copied username: {userPasswordUsername}", userPassword.Username);
        }
        catch (Exception e)
        {
            _logger?.LogError(e, "Failed to copy username");
        }
    }

    [RelayCommand]
    private async Task CopyPassword(UserPassword userPassword)
    {
        try
        {
            // todo: copy decrypted password to clipboard --> how to reset after a specific time?
            _logger.LogInformation("Copy password requested for: {userPasswordTitle}", userPassword.Title);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to copy password");
        }
    }

    [RelayCommand]
    private async Task EditUserPassword(UserPassword userPassword)
    {
        try
        {
            // TODO: open a dialog
            var newTitle = userPassword.Title + " (edited)";
            var newUsername = userPassword.Username;

            await _passwordService.EditPasswordAsync(
                userPassword.Id,
                newTitle,
                newUsername
            );

            await RefreshPasswordsAsync();

            _logger.LogInformation("Edited password: {userPasswordTitle}", userPassword.Title);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to edit user password");
        }
    }

    [RelayCommand]
    private async Task DeleteUserPassword(UserPassword userPassword)
    {
        try
        {
            await _passwordService.DeletePasswordAsync(userPassword.Id);

            UserPasswords.Remove(userPassword);
            FilteredPasswords.Remove(userPassword);

            _logger.LogInformation($"Deleted password: {userPassword.Title}");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to delete user password");
        }
    }
}