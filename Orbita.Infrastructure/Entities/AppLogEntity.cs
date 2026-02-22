namespace Orbita.Infrastructure.Entities;

public class AppLogEntity
{
    public Guid Id { get; set; }
    public string Level { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string? Exception { get; set; }
    public string? StackTrace { get; set; }
    public string? Source { get; set; }
    public string? TraceId { get; set; }
    public DateTime CreatedAt { get; set; }
}
