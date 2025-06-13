using LokPass.Core.Logging;
using Microsoft.Extensions.Logging;

namespace LokPass.Core.Settings;

public class SettingsManager : ISettingsManager
{
    public LogLevel LogLevel { get; set; } = LogLevel.None;
    public string LogFilePath { get; set; } =
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "lokpass.log");

    /// <summary>
    /// Changes the logging level of the logger. Can be changed while the logger is running.
    /// </summary>
    /// <param name="logLevel"></param>
    public void SetLoggingLevel(LogLevel logLevel)
    {
        // todo: how to retrigger the Logger? --> create a function inside file logger to handle this ?
        LogLevel = logLevel;
    }
    
    /// <summary>
    /// Changes the logfile path. Can be changed while the logger is running.
    /// </summary>
    /// <param name="logFilePath"></param>
    public void SetLogFilePath(string logFilePath)
    {
        LogFilePath = logFilePath;
    }
}