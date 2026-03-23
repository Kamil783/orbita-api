namespace Orbita.Contracts.ApiDto.Tasks.Requests;

public sealed class MoveBacklogTaskToWeekRequest
{
    public Guid TargetStatus { get; init; } = default!;
}
