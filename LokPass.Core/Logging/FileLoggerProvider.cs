using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;

namespace LokPass.Core.Logging;

public class FileLoggerProvider(string filePath) : ILoggerProvider
{
    private readonly BlockingCollection<string> _logQueue = new(1024); // todo: 1024 enough / good size?

    public ILogger CreateLogger(string categoryName)
    {
        return new AsyncFileLogger(filePath, categoryName, _logQueue);
    }

    public void Dispose()
    {
        _logQueue.CompleteAdding();
    }
}