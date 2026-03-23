namespace Orbita.Contracts.ApiDto.Tasks.Requests;

public sealed class MoveTaskRequest
{
    public Guid TaskId { get; init; }
    public Guid FromColumnId { get; init; }
    public Guid ToColumnId { get; init; }
    public int FromIndex { get; init; }
    public int ToIndex { get; init; }
}
