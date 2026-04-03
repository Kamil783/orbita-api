using Microsoft.EntityFrameworkCore.Storage;
using Orbita.Application.Abstractions;
using Orbita.Application.Models.Results;

namespace Orbita.Infrastructure.Persistence;

public class UnitOfWork(OrbitaDbContext db) : IUnitOfWork
{
    private IDbContextTransaction? _transaction;

    public async Task BeginTransactionAsync(CancellationToken ct = default)
    {
        if (_transaction is not null)
            throw new InvalidOperationException("Transaction is already started.");

        _transaction = await db.Database.BeginTransactionAsync(ct);
    }

    public async Task CommitAsync(CancellationToken ct = default)
    {
        if (_transaction is null)
            throw new InvalidOperationException("No active transaction to commit.");

        await _transaction.CommitAsync(ct);
        await _transaction.DisposeAsync();
        _transaction = null;
    }

    public async Task RollbackAsync(CancellationToken ct = default)
    {
        if (_transaction is null)
            throw new InvalidOperationException("No active transaction to rollback.");

        await _transaction.RollbackAsync(ct);
        await _transaction.DisposeAsync();
        _transaction = null;
    }

    public async Task<Result<T>> ExecuteAsync<T>(Func<CancellationToken, Task<Result<T>>> action, CancellationToken ct = default)
    {
        await BeginTransactionAsync(ct);

        try
        {
            var result = await action(ct);

            if (result.IsSuccess)
            {
                await CommitAsync(ct);
            }
            else if (_transaction is not null)
            {
                await RollbackAsync(ct);
            }

            return result;
        }
        catch
        {
            if (_transaction is not null)
                await RollbackAsync(ct);

            throw;
        }
    }
}
