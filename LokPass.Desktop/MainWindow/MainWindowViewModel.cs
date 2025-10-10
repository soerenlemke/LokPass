using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Input.Platform;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LokPass.Core.Password;
using LokPass.Core.Password.Crypto;
using LokPass.Core.Password.Repositories;
using LokPass.Core.TestData;
using LokPass.Desktop.Domain.Clipboard;
using LokPass.Desktop.PasswordEditWindow;
using Microsoft.Extensions.Logging;

namespace LokPass.Desktop.MainWindow;

public partial class MainWindowViewModel : ViewModelBase
{
    private readonly IClipboard? _clipboard;
    private readonly ICryptoService _cryptoService;
    private readonly ILogger<MainWindowViewModel> _logger;
    private readonly IPasswordService _passwordService;
    private readonly IClipboardService _clipboardService;

    [ObservableProperty] private ObservableCollection<UserPassword> _filteredPasswords = [];

    [ObservableProperty] private string _newPassword = "";
    [ObservableProperty] private string _newTitle = "";
    [ObservableProperty] private string _newUsername = "";

    [ObservableProperty] private string _popupMessage = "";
    [ObservableProperty] private string _searchText = "";

    private bool _showPasswordFlag;
    [ObservableProperty] private UserConfiguration _userConfiguration;
    [ObservableProperty] private ObservableCollection<UserPassword> _userPasswords = [];

    // Parameterless constructor for designer
    public MainWindowViewModel()
    {
        // TODO: create user configuration with GenerateUserMasterKey(string password) from CryptoService
        _userConfiguration = TestDataService.CreateTestUserConfiguration();

        // For designer - use InMemory Repository
        var repository = new InMemoryPasswordRepository();
        _cryptoService = new CryptoService();
        _passwordService = new PasswordService(repository, _cryptoService);
        _clipboardService = new ClipboardService(_clipboard!);
        _logger = null!;

        // load test data
        FilteredPasswords = [];
        _ = LoadTestDataAsync();
    }

    public MainWindowViewModel(
        ILogger<MainWindowViewModel> logger,
        IPasswordService passwordService,
        ICryptoService cryptoService,
        UserConfiguration userConfiguration,
        IClipboard? clipboard, 
        IClipboardService clipboardService)
    {
        _logger = logger;
        _passwordService = passwordService;
        _cryptoService = cryptoService;
        _userConfiguration = userConfiguration;
        _clipboard = clipboard;
        _clipboardService = clipboardService;

        if (_clipboard is null)
        {
            PopupMessage = "⚠️ Clipboard is not available. Copy functions will not work.";
            _logger.LogError("Clipboard is not available at application startup");
        }

        _logger.LogInformation("MainWindowViewModel constructed!");

        _ = LoadPasswordsAsync();
    }

    partial void OnSearchTextChanged(string value)
    {
        FilterPasswords();
    }

    private async Task LoadTestDataAsync()
    {
        try
        {
            await _passwordService.AddNewPasswordAsync(UserConfiguration, "test title", "test username",
                "980u23urndjofsndnf");

            var passwords = await _passwordService.GetAllPasswordsAsync();
            FilteredPasswords = new ObservableCollection<UserPassword>(passwords);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error loading test data: {ex.Message}");
        }
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
            UpdateObservableCollection(passwords);

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
            await _passwordService.AddNewPasswordAsync(UserConfiguration, NewTitle, NewUsername, NewPassword);

            NewTitle = "";
            NewUsername = "";
            NewPassword = "";
            await RefreshPasswordsAsync();

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
            if (_showPasswordFlag)
            {
                UpdateObservableCollection(UserPasswords);
            }
            else
            {
                var passwords = await _passwordService.GetAllPasswordsAsync();
                UpdateObservableCollection(passwords);
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to refresh passwords");
        }

        _showPasswordFlag = false;
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
    private void OpenSettings()
    {
        _logger.LogInformation("Open settings");
    }

    // PasswordListItem commands

    [RelayCommand]
    private async Task ShowPassword(UserPassword userPassword)
    {
        try
        {
            _showPasswordFlag = true;

            if (userPassword.DecryptedPassword == "*****")
            {
                userPassword.DecryptedPassword =
                    await _cryptoService.DecryptPasswordAsync(userPassword.EncryptedPassword, UserConfiguration);

                _logger.LogInformation("Show password requested for: {userPasswordTitle}", userPassword.Title);
            }
            else
            {
                userPassword.DecryptedPassword = "*****";
                _logger.LogInformation("Hide password requested for: {userPasswordTitle}", userPassword.Title);
            }

            await RefreshPasswordsAsync();
        }
        catch (Exception e)
        {
            userPassword.DecryptedPassword = "*****";
            _showPasswordFlag = false;
            _logger.LogError(e, "Failed to show password");
        }
    }

    [RelayCommand]
    private async Task CopyUsername(UserPassword userPassword)
    {
        try
        {
            if (_clipboard is null)
            {
                PopupMessage = "⚠️ Clipboard is not available. Cannot copy username.";
                _logger.LogWarning("Attempted to copy username but clipboard is not available");
                return;
            }

            await _clipboardService.SetAutoResetValueAsync(userPassword.Username, 10000);
            PopupMessage = "✓ Username copied to clipboard!";
            _logger.LogInformation("Copied username: {userPasswordUsername}", userPassword.Username);

            // Clear success message after 3 seconds
            _ = Task.Delay(3000)
                .ContinueWith<string>(_ => PopupMessage = ""); // introducce MessageService for popups etc
        }
        catch (Exception e)
        {
            PopupMessage = "❌ Failed to copy username to clipboard.";
            _logger.LogError(e, "Failed to copy username");
        }
    }

    [RelayCommand]
    private async Task CopyPassword(UserPassword userPassword)
    {
        try
        {
            if (_clipboard is null)
            {
                PopupMessage = "⚠️ Clipboard is not available. Cannot copy password.";
                _logger.LogWarning("Attempted to copy password but clipboard is not available");
                return;
            }

            var decryptedPassword = await _cryptoService.DecryptPasswordAsync(
                userPassword.EncryptedPassword,
                UserConfiguration);

            await _clipboardService.SetAutoResetValueAsync(decryptedPassword, 10000);
            PopupMessage = "✓ Password copied to clipboard!";
            _logger.LogInformation("Copied password for: {userPasswordTitle}", userPassword.Title);

            // Clear success message after 3 seconds
            _ = Task.Delay(3000)
                .ContinueWith<string>(_ => PopupMessage = ""); // introducce MessageService for popups etc
        }
        catch (Exception e)
        {
            PopupMessage = "❌ Failed to copy password to clipboard.";
            _logger.LogError(e, "Failed to copy password");
        }
    }

    [RelayCommand]
    private async Task EditUserPassword(UserPassword userPassword)
    {
        var mainWindow = GetMainWindow();
        if (mainWindow == null)
        {
            _logger.LogError("Cannot show dialog: Main window is null");
            return;
        }

        var dialog = new EditPasswordDialog
        {
            DataContext = new EditPasswordDialogViewModel(
                UserConfiguration,
                userPassword,
                _passwordService,
                _cryptoService)
        };

        var result = await dialog.ShowDialog<bool?>(mainWindow);
        if (result == true) await RefreshPasswordsAsync();
    }

    private Window? GetMainWindow()
    {
        return Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop
            ? desktop.MainWindow
            : null;
    }

    [RelayCommand]
    private async Task DeleteUserPassword(UserPassword userPassword)
    {
        try
        {
            await _passwordService.DeletePasswordAsync(userPassword.Id);

            UserPasswords.Remove(userPassword);
            FilteredPasswords.Remove(userPassword);

            _logger.LogInformation("Deleted password: {userPassword.Title}", userPassword.Title);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to delete user password");
        }
    }
}