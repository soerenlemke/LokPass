using System;
using CommunityToolkit.Mvvm.ComponentModel;
using LokPass.Core.Password.Hashing;

namespace LokPass.Desktop.LoginWindow;

public partial class LoginDialogViewModel : ViewModelBase
{
    private readonly PasswordHasher _passwordHasher = new();
    
    [ObservableProperty] private string _masterPassword = "";

    private string LoadSavedMasterPassword()
    {
        throw new NotImplementedException();
    }

    private bool CheckPassword()
    {
        throw new NotImplementedException();
        // _passwordHasher.IsValidPasswordAsync(_masterPassword, /* saved hashed password */);
    }
}