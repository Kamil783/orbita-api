namespace Orbita.Application.Abstractions.Jobs;

public interface IDailyJob
{
    string Name { get; }
    Task ExecuteAsync(CancellationToken ct);
}
