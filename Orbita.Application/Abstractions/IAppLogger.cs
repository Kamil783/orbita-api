using Orbita.Application.Models.Log;

namespace Orbita.Application.Abstractions;

public interface IAppLogger
{
    void LogDebug(string message, string? source = null);
    void LogInfo(string message, string? source = null);
    void LogWarning(string message, string? source = null);
    void LogError(string message, Exception? exception = null, string? source = null);
    void LogCritical(string message, Exception? exception = null, string? source = null);
    Task LogRequestAsync(RequestLogEntry entry);
}
