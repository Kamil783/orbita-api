namespace Orbita.Application.Models;

public class RequestLogEntry
{
    public string TraceId { get; init; } = string.Empty;
    public string HttpMethod { get; init; } = string.Empty;
    public string Path { get; init; } = string.Empty;
    public string? QueryString { get; init; }
    public string? RequestBody { get; init; }
    public string? RequestHeaders { get; init; }
    public int ResponseStatusCode { get; init; }
    public string? ResponseBody { get; init; }
    public string? UserId { get; init; }
    public string? ClientIp { get; init; }
    public string? UserAgent { get; init; }
    public string? ControllerName { get; init; }
    public string? ActionName { get; init; }
    public long DurationMs { get; init; }
}
