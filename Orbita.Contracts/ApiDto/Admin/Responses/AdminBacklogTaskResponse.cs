namespace Orbita.Contracts.ApiDto.Admin.Responses;

public sealed class AdminBacklogTaskResponse
{
    public string Id { get; init; } = default!;
    public string Title { get; init; } = default!;
    public string Priority { get; init; } = default!;
    public bool IsCompleted { get; init; }
    public bool InWeek { get; init; }
    public string CreatorId { get; init; } = default!;
    public string? CreatorName { get; init; }
    public string[]? AssigneeIds { get; init; }
    public string? DueDate { get; init; }
    public string? CreatedAt { get; init; }
}
