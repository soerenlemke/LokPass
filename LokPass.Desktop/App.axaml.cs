using System;
using System.IO;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using System.Linq;
using Avalonia.Markup.Xaml;
using LokPass.Core.Logging;
using LokPass.Desktop.ViewModels;
using LokPass.Desktop.Views;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace LokPass.Desktop;
public partial class App : Application
{
    public static IServiceProvider Services { get; private set; } = null!;
    
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        var services = new ServiceCollection();
        
        // todo: make this adjustable inside the apps settings
        var logPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "lokpass.log");
        
        services.AddLogging(configure =>
        {
            configure.ClearProviders();
            configure.AddProvider(new FileLoggerProvider(logPath));
            configure.SetMinimumLevel(LogLevel.Information);
        });
        
        Services = services.BuildServiceProvider();
        
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            // Avoid duplicate validations from both Avalonia and the CommunityToolkit. 
            // More info: https://docs.avaloniaui.net/docs/guides/development-guides/data-validation#manage-validationplugins
            DisableAvaloniaDataAnnotationValidation();
            
            var logger = Services.GetRequiredService<ILogger<MainWindowViewModel>>();
            desktop.MainWindow = new MainWindow
            {
                DataContext = new MainWindowViewModel(logger),
            };
        }

        base.OnFrameworkInitializationCompleted();
    }

    private void DisableAvaloniaDataAnnotationValidation()
    {
        // Get an array of plugins to remove
        var dataValidationPluginsToRemove =
            BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();

        // remove each entry found
        foreach (var plugin in dataValidationPluginsToRemove)
        {
            BindingPlugins.DataValidators.Remove(plugin);
        }
    }
}
