using LokPass.Core.Logging;
using Microsoft.Extensions.Logging;

namespace LokPass.Core.Settings;

public class SettingsManager : ISettingsManager
{
    public LogLevel LogLevel { get; set; } = LogLevel.None;
    public string LogFilePath { get; set; } =
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "lokpass.log");

    public void SetLoggingMode(LogLevel logLevel)
    {
        // todo: how to retrigger the Logger? --> create a function inside file logger to handle this ?
        LogLevel = logLevel;
    }
    
    public void SetLogFilePath(string logFilePath)
    {
        LogFilePath = logFilePath;
    }
}