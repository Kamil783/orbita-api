namespace Orbita.Infrastructure.Entities;

public class RequestLogEntity
{
    public Guid Id { get; set; }
    public string TraceId { get; set; } = string.Empty;
    public string HttpMethod { get; set; } = string.Empty;
    public string Path { get; set; } = string.Empty;
    public string? QueryString { get; set; }
    public string? RequestBody { get; set; }
    public string? RequestHeaders { get; set; }
    public int ResponseStatusCode { get; set; }
    public string? ResponseBody { get; set; }
    public string? UserId { get; set; }
    public string? ClientIp { get; set; }
    public string? UserAgent { get; set; }
    public string? ControllerName { get; set; }
    public string? ActionName { get; set; }
    public long DurationMs { get; set; }
    public DateTime CreatedAt { get; set; }
}
