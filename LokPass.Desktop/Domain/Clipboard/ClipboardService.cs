using System;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Avalonia.Input.Platform;
using Timer = System.Timers.Timer;

namespace LokPass.Desktop.Domain.Clipboard;

public sealed class ClipboardService : IClipboardService, IDisposable
{
    private readonly IClipboard _clipboard;
    private readonly Timer _timer;
    private string? _lastSetValue;
    private readonly Lock _lock = new();

    public ClipboardService(IClipboard clipboard)
    {
        _clipboard = clipboard ?? throw new ArgumentNullException(nameof(clipboard));

        _timer = new Timer
        {
            AutoReset = false,
            Enabled = false
        };
        _timer.Elapsed += OnTimerElapsed;
    }

    public async Task<string?> GetValueAsync()
        => await _clipboard.GetTextAsync().ConfigureAwait(false);

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

    private async void OnTimerElapsed(object? sender, ElapsedEventArgs e)
    {
        try
        {
            string? expected;
            lock (_lock) { expected = _lastSetValue; }

            try
            {
                var current = await _clipboard.GetTextAsync().ConfigureAwait(false);
                if (string.Equals(current, expected, StringComparison.Ordinal))
                {
                    await _clipboard.ClearAsync().ConfigureAwait(false);
                }
            }
            catch
            {
                // Logging optional einbauen – dont throw exceptions to ui
            }
            finally
            {
                lock (_lock) { _lastSetValue = null; }
            }
        }
        catch (Exception ex)
        {
            throw; // TODO handle exception
        }
    }

    public void Dispose()
    {
        _timer.Elapsed -= OnTimerElapsed;
        _timer.Dispose();
    }
}
