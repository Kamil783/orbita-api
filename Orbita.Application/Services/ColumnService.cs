using Orbita.Application.Abstractions.Repositories;
using Orbita.Application.Abstractions.Services;
using Orbita.Application.Models.Results;
using Orbita.Domain.Entities;
using Orbita.Domain.Enums;
using Orbita.Domain.ValueObjects;

namespace Orbita.Application.Services;

public class ColumnService(IColumnRepository repository, ITeamProvider teamProvider) : IColumnService
{
    public async Task<Result<Column>> CreateAsync(Guid userId, string title, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(title))
            return Result<Column>.Fail("Title is required.", ErrorType.Validation);

        var teamId = await teamProvider.GetTeamIdAsync(userId, ct);

        var column = Column.Create(
            title: title,
            status: TodoItemStatus.Unclassified,
            headerActionIcon: "add_circle",
            teamId: new TeamId(teamId),
            creatorId: new UserId(userId));

        var created = await repository.CreateAsync(column, ct);

        return Result<Column>.Ok(created);
    }
}
