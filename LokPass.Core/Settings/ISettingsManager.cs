using LokPass.Core.Logging;
using Microsoft.Extensions.Logging;

namespace LokPass.Core.Settings;

public interface ISettingsManager
{
    LogLevel LogLevel { get; set; }
    string LogFilePath { get; set; }
    void SetLoggingLevel(LogLevel logLevel);
    void SetLogFilePath(string logFilePath);
}