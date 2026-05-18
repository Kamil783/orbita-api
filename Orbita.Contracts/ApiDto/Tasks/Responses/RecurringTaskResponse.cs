namespace Orbita.Contracts.ApiDto.Tasks.Responses;

public sealed class RecurringTaskResponse
{
    public string Id { get; init; } = default!;
    public string Title { get; init; } = default!;
    public string? Description { get; init; }
    public int DayOfMonth { get; init; }
    public bool IsCompleted { get; init; }
    public long CreatedAt { get; init; }
    public long UpdatedAt { get; init; }
}
