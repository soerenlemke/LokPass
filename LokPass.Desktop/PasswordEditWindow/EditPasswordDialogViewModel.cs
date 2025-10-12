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
    private readonly UserConfiguration _userConiguration;
    private readonly UserPassword _userPassword;

    [ObservableProperty] private string _newPassword = "";
    [ObservableProperty] private string _newTitle = "";
    [ObservableProperty] private string _newUsername = "";

    public EditPasswordDialogViewModel()
    {
        _userConiguration = new UserConfiguration(
            new byte[32],
            new byte[16]
        );
        _cryptoService = new CryptoService();
        _passwordService = null!;

        var emptyEncryptedPassword = new EncryptedPassword(
            [],
            []
        );

        _userPassword = new UserPassword(
            Guid.NewGuid(),
            "test password",
            "test user",
            emptyEncryptedPassword,
            DateTime.UtcNow,
            null
        );
    }

    public EditPasswordDialogViewModel(
        UserConfiguration userConfiguration,
        UserPassword userPassword,
        IPasswordService passwordService,
        ICryptoService cryptoService)
    {
        _userConiguration = userConfiguration;
        _userPassword = userPassword;
        _passwordService = passwordService;
        _cryptoService = cryptoService;

        NewTitle = userPassword.Title;
        NewUsername = userPassword.Username;

        _ = LoadPasswordAsync();
    }

    private async Task LoadPasswordAsync()
    {
        try
        {
            NewPassword = await _cryptoService.DecryptPasswordAsync(
                _userPassword.EncryptedPassword,
                _userConiguration);
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
            _userConiguration,
            _userPassword.Id,
            NewTitle,
            NewUsername,
            string.IsNullOrEmpty(NewPassword) ? null : NewPassword);

        CloseRequested?.Invoke(this, true);
    }
}