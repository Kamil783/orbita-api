using Orbita.Domain.Enums;
using Orbita.Domain.ValueObjects;

namespace Orbita.Domain.Entities;

public class Column
{
    public ColumnId Id { get; private set; }
    public string Title { get; private set; }
    public int TotalCount { get; private set; }
    public string HeaderActionIcon { get; private set; }
    public TodoItemStatus Status { get; private set; }
    public bool Muted { get; private set; }

    private readonly List<TodoItem> _todoItems = [];
    public IReadOnlyCollection<TodoItem> TodoItems => _todoItems.AsReadOnly();
}
