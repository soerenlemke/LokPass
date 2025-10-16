using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LokPass.Core.Password;
using LokPass.Core.Password.Crypto;

namespace LokPass.Desktop.PasswordEditWindow;

public partial class EditPasswordDialogViewModel : ViewModelBase
{
    private readonly ICryptoService _cryptoService;
    private readonly IPasswordService _passwordService;
    private readonly UserConfiguration _userConfiguration;
    private readonly UserPasswordView _userPassword;

    [ObservableProperty] private string _newPassword = "";
    [ObservableProperty] private string _newTitle = "";
    [ObservableProperty] private string _newUsername = "";
    
    public UserPasswordView Result { get; private set; }

    public EditPasswordDialogViewModel()
    {
        _userConfiguration = new UserConfiguration(
            new byte[32],
            new byte[16],
            DateTime.Now
        );
        _cryptoService = new CryptoService();
        _passwordService = null!;

        var emptyEncryptedPassword = new EncryptedPassword(
            [],
            []
        );

        var userPassword = new UserPassword(
            Guid.NewGuid(),
            "test title",
            "test user",
            emptyEncryptedPassword,
            DateTime.UtcNow,
            null
        );
        
        _userPassword = new UserPasswordView(userPassword);
        Result = _userPassword;
    }

    public EditPasswordDialogViewModel(
        UserConfiguration userConfiguration,
        UserPasswordView userPassword,
        IPasswordService passwordService,
        ICryptoService cryptoService)
    {
        _userConfiguration = userConfiguration;
        _userPassword = userPassword;
        _passwordService = passwordService;
        _cryptoService = cryptoService;

        NewTitle = userPassword.Password.Title;
        NewUsername = userPassword.Password.Username;
        Result = _userPassword;

        _ = LoadPasswordAsync();
    }

    private async Task LoadPasswordAsync()
    {
        try
        {
            NewPassword = await _cryptoService.DecryptPasswordAsync(
                _userPassword.Password.EncryptedPassword,
                _userConfiguration);
        }
        catch (Exception)
        {
            NewPassword = string.Empty;
        }
    }

    public event EventHandler<bool>? CloseRequested;

    [RelayCommand]
    private void Cancel()
    {
        CloseRequested?.Invoke(this, false);
    }

    [RelayCommand]
    private async Task Save()
    {
        await _passwordService.EditPasswordAsync(
            _userConfiguration,
            _userPassword.Password.Id,
            NewTitle,
            NewUsername,
            string.IsNullOrWhiteSpace(NewPassword) ? null : NewPassword);

        var savedPassword = await _passwordService.GetPasswordByIdAsync(_userPassword.Password.Id);

        if (savedPassword == null)
        {
            CloseRequested?.Invoke(this, false);
            return;
        }

        Result = new UserPasswordView(
            Password: savedPassword,
            DecryptedPassword: string.IsNullOrWhiteSpace(NewPassword)
                ? _userPassword.DecryptedPassword
                : NewPassword
        );

        CloseRequested?.Invoke(this, true);
    }
}