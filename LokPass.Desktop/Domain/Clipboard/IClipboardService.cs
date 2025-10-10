using System.Threading;
using System.Threading.Tasks;

namespace LokPass.Desktop.Domain.Clipboard;

public interface IClipboardService
{
    public Task<string?> GetValueAsync();
    public Task SetAutoResetValueAsync(string value, int millisecondsDelay = 3000, CancellationToken ct = default);
}