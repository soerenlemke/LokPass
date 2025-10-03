using System;
using SukiUI.Controls;

namespace LokPass.Desktop.PasswordEditWindow;

public partial class EditPasswordDialog : SukiWindow
{
    private EditPasswordDialogViewModel? _currentViewModel;

    public EditPasswordDialog()
    {
        InitializeComponent();

        DataContextChanged += OnDataContextChanged;
    }

    private void OnDataContextChanged(object? sender, EventArgs e)
    {
        if (_currentViewModel != null) _currentViewModel.CloseRequested -= OnCloseRequested;

        if (DataContext is EditPasswordDialogViewModel viewModel)
        {
            viewModel.CloseRequested += OnCloseRequested;
            _currentViewModel = viewModel;
        }
        else
        {
            _currentViewModel = null;
        }
    }

    private void OnCloseRequested(object? sender, bool result)
    {
        Close(result);
    }

    protected override void OnClosed(EventArgs e)
    {
        if (_currentViewModel != null)
        {
            _currentViewModel.CloseRequested -= OnCloseRequested;
            _currentViewModel = null;
        }

        DataContextChanged -= OnDataContextChanged;

        base.OnClosed(e);
    }
}