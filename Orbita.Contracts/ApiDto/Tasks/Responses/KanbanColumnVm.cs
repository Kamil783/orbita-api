namespace Orbita.Contracts.ApiDto.Tasks.Responses;

public sealed class KanbanColumnVm
{
    public string Id { get; init; } = default!;
    public string Title { get; init; } = default!;
    public int TotalCount { get; init; }
    public string ColumnType { get; init; } = default!;
    public string HeaderActionIcon { get; init; } = default!;
    public bool? Muted { get; init; }
    public List<TaskCardVm> Cards { get; init; } = [];
}
