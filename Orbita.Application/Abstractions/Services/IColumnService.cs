using Orbita.Application.Models.Results;
using Orbita.Domain.Entities;

namespace Orbita.Application.Abstractions.Services;

public interface IColumnService
{
    Task<Result<Column>> CreateAsync(string title, CancellationToken ct = default);
}
