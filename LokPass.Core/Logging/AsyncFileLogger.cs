using System.Collections.Concurrent;
using LokPass.Core.Settings;
using Microsoft.Extensions.Logging;

namespace LokPass.Core.Logging;

public class AsyncFileLogger : ILogger
{
    private readonly ISettingsManager _settingsManager;
    private static bool _isConsumerStarted;
    private static readonly Lock StartLock = new();
    private readonly string _categoryName;
    private readonly BlockingCollection<string> _logQueue;

    public AsyncFileLogger(
        ISettingsManager settingsManager,
        string categoryName,
        BlockingCollection<string> logQueue
    )
    {
        _settingsManager = settingsManager;
        _categoryName = categoryName;
        _logQueue = logQueue;

        StartConsumerOnce();
    }

    public IDisposable BeginScope<TState>(TState state)
    {
        return null!;
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        return true;
    }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception,
        Func<TState, Exception?, string> formatter)
    {
        if (!IsEnabled(logLevel)) return;
        var logRecord =
            $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} [{logLevel}] {_categoryName}: {formatter(state, exception)}";
        if (exception != null)
            logRecord += Environment.NewLine + exception;

        _logQueue.Add(logRecord);
    }

    private void StartConsumerOnce()
    {
        lock (StartLock)
        {
            if (_isConsumerStarted) return;
            _isConsumerStarted = true;
            Task.Run(LogConsumer);
        }
    }

    private void LogConsumer()
    {
        foreach (var log in _logQueue.GetConsumingEnumerable())
            try
            {
                File.AppendAllText(_settingsManager.LogFilePath, log + Environment.NewLine);
            }
            catch (Exception e)
            {
                // todo: retry logic?
            }
    }
}