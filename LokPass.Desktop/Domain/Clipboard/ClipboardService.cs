using System;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Avalonia.Input.Platform;
using Microsoft.Extensions.Logging;
using Timer = System.Timers.Timer;

namespace LokPass.Desktop.Domain.Clipboard;

public sealed class ClipboardService : IClipboardService, IDisposable
{
    private readonly IClipboard _clipboard;
    private readonly Lock _lock = new();
    private readonly ILogger _logger;

    private readonly Timer _timer;
    private string? _lastSetValue;

    public ClipboardService(IClipboard clipboard, ILogger logger)
    {
        _clipboard = clipboard ?? throw new ArgumentNullException(nameof(clipboard));
        _logger = logger;

        _timer = new Timer
        {
            AutoReset = false,
            Enabled = false
        };
        _timer.Elapsed += OnTimerElapsed;
    }

    public async Task<string?> GetValueAsync()
    {
        return await _clipboard.GetTextAsync().ConfigureAwait(false);
    }

    public async Task SetAutoResetValueAsync(string value, int millisecondsDelay = 3000, CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();

        await _clipboard.SetTextAsync(value).ConfigureAwait(false);

        lock (_lock)
        {
            _lastSetValue = value;
            _timer.Interval = Math.Max(1, millisecondsDelay);
            _timer.Stop();
            _timer.Start();
        }
    }

    public void Dispose()
    {
        _timer.Elapsed -= OnTimerElapsed;
        _timer.Dispose();
    }

    private async void OnTimerElapsed(object? sender, ElapsedEventArgs e)
    {
        try
        {
            string? expected;
            lock (_lock)
            {
                expected = _lastSetValue;
            }

            try
            {
                var current = await _clipboard.GetTextAsync().ConfigureAwait(false);
                if (string.Equals(current, expected, StringComparison.Ordinal))
                    await _clipboard.ClearAsync().ConfigureAwait(false);
            }
            catch
            {
                _logger.LogError("Failed to set clipboard");
            }
            finally
            {
                lock (_lock)
                {
                    _lastSetValue = null;
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to set clipboard");
        }
    }
}