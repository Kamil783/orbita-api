using Orbita.Domain.Enums;

namespace Orbita.Infrastructure.Entities;

public class ColumnEntity
{
    public Guid Id { get; set; }
    public string Title { get; set; } = default!;
    public int TotalCount { get; set; }
    public string HeaderActionIcon { get; set; } = default!;
    public TodoItemStatus Status { get; set; }
    public bool Muted { get; set; }

    public List<TodoItemEntity> TodoItems { get; set; } = new();
}
