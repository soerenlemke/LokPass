using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using LokPass.Core.Password;
using LokPass.Core.Password.Crypto;
using LokPass.Core.Password.Repositories;
using LokPass.Core.TestData;
using LokPass.Desktop.ViewModels;
using LokPass.Desktop.Views;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;

namespace LokPass.Desktop;

public class App : Application
{
    public static IServiceProvider Services { get; private set; } = null!;

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day)
            .CreateLogger();

        var services = new ServiceCollection();

        // Configure logging
        services.AddLogging(loggingBuilder =>
        {
            loggingBuilder.ClearProviders();
            loggingBuilder.AddSerilog();
        });

        // Register your password services
        ConfigurePasswordServices(services);

        // Build the service provider
        Services = services.BuildServiceProvider();

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            // Avoid duplicate validations from both Avalonia and the CommunityToolkit. 
            // More info: https://docs.avaloniaui.net/docs/guides/development-guides/data-validation#manage-validationplugins
            DisableAvaloniaDataAnnotationValidation();

            var logger = Services.GetRequiredService<ILogger<MainWindowViewModel>>();
            var passwordService = Services.GetRequiredService<IPasswordService>();
            var cryptoService = Services.GetRequiredService<ICryptoService>();
            var userConfiguration = TestDataService.CreateTestUserConfiguration();

            var mainWindow = new MainWindow();
            desktop.MainWindow = mainWindow;

            mainWindow.DataContext = new MainWindowViewModel(
                logger,
                passwordService,
                cryptoService,
                userConfiguration,
                mainWindow.Clipboard);
        }

        base.OnFrameworkInitializationCompleted();
    }

    private void ConfigurePasswordServices(IServiceCollection services)
    {
        // Register core password services
        services.AddSingleton<ICryptoService, CryptoService>();
        services.AddSingleton<IPasswordRepository, InMemoryPasswordRepository>();
        services.AddSingleton<IPasswordService, PasswordService>();

        // Register ViewModels if needed
        services.AddTransient<MainWindowViewModel>();
    }

    private void DisableAvaloniaDataAnnotationValidation()
    {
        // Get an array of plugins to remove
        var dataValidationPluginsToRemove =
            BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();

        // remove each entry found
        foreach (var plugin in dataValidationPluginsToRemove) BindingPlugins.DataValidators.Remove(plugin);
    }
}