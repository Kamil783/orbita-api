using System.Threading.Channels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Orbita.Infrastructure.Entities;
using Orbita.Infrastructure.Persistence;

namespace Orbita.Infrastructure.Logging;

public class LogBackgroundService : BackgroundService
{
    private readonly Channel<AppLogEntity> _channel;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<LogBackgroundService> _logger;

    public LogBackgroundService(
        Channel<AppLogEntity> channel,
        IServiceScopeFactory scopeFactory,
        ILogger<LogBackgroundService> logger)
    {
        _channel = channel;
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Log background service started");

        while (await _channel.Reader.WaitToReadAsync(stoppingToken))
        {
            var batch = new List<AppLogEntity>();

            while (_channel.Reader.TryRead(out var item))
            {
                batch.Add(item);
                if (batch.Count >= 50)
                    break;
            }

            if (batch.Count == 0)
                continue;

            try
            {
                using var scope = _scopeFactory.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<OrbitaDbContext>();
                db.AppLogs.AddRange(batch);
                await db.SaveChangesAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to write {Count} app log entries to database", batch.Count);
            }
        }

        _logger.LogInformation("Log background service stopped");
    }
}
