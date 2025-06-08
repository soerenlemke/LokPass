using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LokPass.Core;

namespace LokPass.Desktop.ViewModels;
public partial class MainWindowViewModel : ViewModelBase
{
    [ObservableProperty]
    private ObservableCollection<UserPassword> _userPasswords = [];
    
    [ObservableProperty]
    private string _newTitle = "";

    [ObservableProperty]
    private string _newUsername = "";

    [ObservableProperty]
    private string _newPassword = "";

    [RelayCommand]
    private void AddUserPassword()
    {
        if (!string.IsNullOrWhiteSpace(NewTitle))
        {
            UserPasswords.Add(
                new UserPassword(NewTitle, NewUsername, NewPassword)
            );

            NewTitle = "";
            NewUsername = "";
            NewPassword = "";

            OnPropertyChanged(nameof(UserPasswords));
        }
    }
}
