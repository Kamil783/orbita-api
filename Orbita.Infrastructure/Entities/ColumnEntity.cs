using Orbita.Domain.Enums;
using Orbita.Domain.ValueObjects;

namespace Orbita.Infrastructure.Entities;

public class ColumnEntity
{
    public ColumnId Id { get; set; } = default!;
    public string Title { get; set; } = default!;
    public int TotalCount { get; set; }
    public string HeaderActionIcon { get; set; } = default!;
    public TodoItemStatus Status { get; set; }
    public bool Muted { get; set; }

    public List<TodoItemEntity> TodoItems { get; set; } = new();
}
