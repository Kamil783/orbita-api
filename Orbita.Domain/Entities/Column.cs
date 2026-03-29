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
    public UserId? CreatorId { get; private set; }
    public TeamId TeamId { get; private set; }

    private readonly List<TodoItem> _todoItems = [];
    public IReadOnlyCollection<TodoItem> TodoItems => _todoItems.AsReadOnly();

    public bool IsDefault => CreatorId is null;

    private Column() { }

    public static Column Create(string title, TodoItemStatus status, string headerActionIcon, bool muted = false, UserId? creatorId = null)
    {
        return new Column
        {
            Id = new ColumnId(Guid.NewGuid()),
            Title = title,
            TotalCount = 0,
            HeaderActionIcon = headerActionIcon,
            Status = status,
            Muted = muted,
            CreatorId = creatorId
        };
    }

    public static Column Restore(
        ColumnId id,
        string title,
        int totalCount,
        string headerActionIcon,
        TodoItemStatus status,
        bool muted,
        UserId? creatorId,
        IEnumerable<TodoItem> todoItems)
    {
        var column = new Column
        {
            Id = id,
            Title = title,
            TotalCount = totalCount,
            HeaderActionIcon = headerActionIcon,
            Status = status,
            Muted = muted,
            CreatorId = creatorId
        };
        column._todoItems.AddRange(todoItems);
        return column;
    }

    public void SetTotalCount(int count)
    {
        TotalCount = count;
    }
}
