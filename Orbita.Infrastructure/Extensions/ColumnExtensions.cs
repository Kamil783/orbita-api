using Orbita.Domain.Entities;
using Orbita.Domain.ValueObjects;
using Orbita.Infrastructure.Entities;

namespace Orbita.Infrastructure.Extensions;

public static class ColumnExtensions
{
    public static ColumnEntity ToEntity(this Column column)
    {
        return new ColumnEntity
        {
            Id = column.Id.Id,
            Title = column.Title,
            TotalCount = column.TotalCount,
            HeaderActionIcon = column.HeaderActionIcon,
            Status = column.Status,
            Muted = column.Muted
        };
    }

    public static Column ToDomain(this ColumnEntity entity)
    {
        var todoItems = entity.TodoItems?
            .Select(t => t.ToDomain())
            .ToList() ?? [];

        return Column.Restore(
            id: new ColumnId(entity.Id),
            title: entity.Title,
            totalCount: entity.TotalCount,
            headerActionIcon: entity.HeaderActionIcon,
            status: entity.Status,
            muted: entity.Muted,
            todoItems: todoItems
        );
    }
}
