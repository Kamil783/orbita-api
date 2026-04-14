namespace Orbita.Contracts.ApiDto.Tasks.Responses;

public sealed class TimeEntryResponse
{
    public string Id { get; init; } = default!;
    public string UserId { get; init; } = default!;
    public int Minutes { get; init; }
    public string? Description { get; init; }
    public long CreatedAt { get; init; }
}
