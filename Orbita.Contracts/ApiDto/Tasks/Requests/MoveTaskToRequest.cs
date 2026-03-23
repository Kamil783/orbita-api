namespace Orbita.Contracts.ApiDto.Tasks.Requests;

public sealed class MoveTaskToRequest
{
    public Guid TaskId { get; init; }
    public Guid TargetStatus { get; init; }
}
