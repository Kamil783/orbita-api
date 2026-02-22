using System.Diagnostics;
using System.Threading.Channels;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Orbita.Application.Abstractions;
using Orbita.Application.Models;
using Orbita.Infrastructure.Entities;
using Orbita.Infrastructure.Persistence;

namespace Orbita.Infrastructure.Logging;

public class AppLogger : IAppLogger
{
    private readonly ILogger<AppLogger> _logger;
    private readonly Channel<AppLogEntity> _appLogChannel;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AppLogger(
        ILogger<AppLogger> logger,
        Channel<AppLogEntity> appLogChannel,
        IServiceScopeFactory scopeFactory,
        IHttpContextAccessor httpContextAccessor)
    {
        _logger = logger;
        _appLogChannel = appLogChannel;
        _scopeFactory = scopeFactory;
        _httpContextAccessor = httpContextAccessor;
    }

    public void LogDebug(string message, string? source = null)
    {
        _logger.LogDebug("[{Source}] {Message}", source ?? "App", message);
        Enqueue("Debug", message, source: source);
    }

    public void LogInfo(string message, string? source = null)
    {
        _logger.LogInformation("[{Source}] {Message}", source ?? "App", message);
        Enqueue("Information", message, source: source);
    }

    public void LogWarning(string message, string? source = null)
    {
        _logger.LogWarning("[{Source}] {Message}", source ?? "App", message);
        Enqueue("Warning", message, source: source);
    }

    public void LogError(string message, Exception? exception = null, string? source = null)
    {
        _logger.LogError(exception, "[{Source}] {Message}", source ?? "App", message);
        Enqueue("Error", message, exception, source);
    }

    public void LogCritical(string message, Exception? exception = null, string? source = null)
    {
        _logger.LogCritical(exception, "[{Source}] {Message}", source ?? "App", message);
        Enqueue("Critical", message, exception, source);
    }

    public async Task LogRequestAsync(RequestLogEntry entry)
    {
        _logger.LogInformation(
            "{Method} {Path}{Query} -> {StatusCode} ({Duration}ms) | User: {UserId} | IP: {ClientIp}",
            entry.HttpMethod,
            entry.Path,
            entry.QueryString,
            entry.ResponseStatusCode,
            entry.DurationMs,
            entry.UserId ?? "anonymous",
            entry.ClientIp);

        var entity = new RequestLogEntity
        {
            Id = Guid.NewGuid(),
            TraceId = entry.TraceId,
            HttpMethod = entry.HttpMethod,
            Path = entry.Path,
            QueryString = entry.QueryString,
            RequestBody = entry.RequestBody,
            RequestHeaders = entry.RequestHeaders,
            ResponseStatusCode = entry.ResponseStatusCode,
            ResponseBody = entry.ResponseBody,
            UserId = entry.UserId,
            ClientIp = entry.ClientIp,
            UserAgent = entry.UserAgent,
            ControllerName = entry.ControllerName,
            ActionName = entry.ActionName,
            DurationMs = entry.DurationMs,
            CreatedAt = DateTime.UtcNow
        };

        try
        {
            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<OrbitaDbContext>();
            db.RequestLogs.Add(entity);
            await db.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to write request log to database");
        }
    }

    private void Enqueue(string level, string message, Exception? exception = null, string? source = null)
    {
        var traceId = Activity.Current?.TraceId.ToString()
                      ?? _httpContextAccessor.HttpContext?.TraceIdentifier;

        var entity = new AppLogEntity
        {
            Id = Guid.NewGuid(),
            Level = level,
            Message = message,
            Exception = exception?.Message,
            StackTrace = exception?.StackTrace,
            Source = source,
            TraceId = traceId,
            CreatedAt = DateTime.UtcNow
        };

        _appLogChannel.Writer.TryWrite(entity);
    }
}
