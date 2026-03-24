using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Orbita.Application.Abstractions.Jobs;

namespace Orbita.Infrastructure.Jobs;

public class DailyTaskRunnerService(
    IServiceScopeFactory scopeFactory,
    ILogger<DailyTaskRunnerService> logger) : BackgroundService
{
    private static readonly TimeSpan CheckInterval = TimeSpan.FromHours(1);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("DailyTaskRunner started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = scopeFactory.CreateScope();
                var jobs = scope.ServiceProvider.GetServices<IDailyJob>();

                foreach (var job in jobs)
                {
                    try
                    {
                        await job.ExecuteAsync(stoppingToken);
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "Daily job '{JobName}' failed", job.Name);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "DailyTaskRunner iteration failed");
            }

            await Task.Delay(CheckInterval, stoppingToken);
        }

        logger.LogInformation("DailyTaskRunner stopped");
    }
}
