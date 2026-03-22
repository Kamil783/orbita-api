using Microsoft.EntityFrameworkCore;
using Orbita.Application.Abstractions.Repositories;
using Orbita.Domain.Entities;
using Orbita.Infrastructure.Extensions;
using Orbita.Infrastructure.Persistence;

namespace Orbita.Infrastructure.Repositories;

public class ColumnRepository(OrbitaDbContext db) : IColumnRepository
{
    public async Task<Column?> GetAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await db.Columns
            .FirstOrDefaultAsync(x => x.Id == id, ct);

        return entity?.ToDomain();
    }

    public async Task<IReadOnlyCollection<Column>> GetAllWithItemsAsync(Guid userId, CancellationToken ct = default)
    {
        var entities = await db.Columns
            .Where(c => c.CreatorId == null || c.CreatorId == userId)
            .Include(c => c.TodoItems
                .Where(t => t.CreatorId == userId)
                .OrderBy(t => t.SortOrder))
            .OrderBy(c => c.Status)
            .ToListAsync(ct);

        return entities.Select(e => e.ToDomain()).ToList();
    }

    public async Task<Column> CreateAsync(Column column, CancellationToken ct = default)
    {
        var entity = column.ToEntity();

        await db.Columns.AddAsync(entity, ct);
        await db.SaveChangesAsync(ct);

        return entity.ToDomain();
    }
}
