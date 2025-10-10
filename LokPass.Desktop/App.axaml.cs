using System;
using System.IO;
using System.Linq;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using LokPass.Core.Password;
using LokPass.Core.Password.Crypto;
using LokPass.Core.Password.Repositories;
using LokPass.Core.TestData;
using LokPass.Desktop.Domain.Clipboard;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using MainWindowViewModel = LokPass.Desktop.MainWindow.MainWindowViewModel;

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

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            // Avoid duplicate validations from both Avalonia and the CommunityToolkit. 
            // More info: https://docs.avaloniaui.net/docs/guides/development-guides/data-validation#manage-validationplugins
            DisableAvaloniaDataAnnotationValidation();

            // 1) Window erzeugen (damit wir das Clipboard haben)
            var mainWindow = new MainWindow.MainWindow();
            desktop.MainWindow = mainWindow;

            // 2) ClipboardService *nach* dem Window registrieren (Factory nutzt mainWindow.Clipboard)
            services.AddSingleton<IClipboardService>(_ => new ClipboardService(mainWindow.Clipboard!));

            // 3) ServiceProvider bauen
            Services = services.BuildServiceProvider();

            // 4) ViewModel aus DI holen und setzen
            var logger = Services.GetRequiredService<ILogger<MainWindowViewModel>>();
            var passwordService = Services.GetRequiredService<IPasswordService>();
            var cryptoService = Services.GetRequiredService<ICryptoService>();
            var userConfiguration = TestDataService.CreateTestUserConfiguration();
            var clipboardService = Services.GetRequiredService<IClipboardService>();

            mainWindow.DataContext = new MainWindowViewModel(
                logger,
                passwordService,
                cryptoService,
                userConfiguration,
                mainWindow.Clipboard,
                clipboardService);
        }

        base.OnFrameworkInitializationCompleted();
    }

    private void ConfigurePasswordServices(IServiceCollection services)
    {
        // Register core password services
        services.AddSingleton<ICryptoService, CryptoService>();

        /*
            File path:
           Windows: C:\Users\[Username]\AppData\Roaming\LokPass\passwords.json
           macOS: /Users/[Username]/.config/LokPass/passwords.json
           Linux: /home/[Username]/.config/LokPass/passwords.json
         */

        var passwordsFilePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "LokPass",
            "passwords.json"
        );

        services.AddSingleton<IPasswordRepository>(provider =>
            new FilePasswordRepository(passwordsFilePath));

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