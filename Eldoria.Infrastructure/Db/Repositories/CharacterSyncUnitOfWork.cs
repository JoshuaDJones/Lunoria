using System.Data;
using Eldoria.Core.Exceptions;
using Eldoria.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Eldoria.Infrastructure.Db.Repositories;

public sealed class CharacterSyncUnitOfWork(ApplicationDbContext db) : ICharacterSyncUnitOfWork
{
    public async Task<T> ExecuteAsync<T>(Func<Task<T>> action, CancellationToken ct)
    {
        await using var transaction = await db.Database.BeginTransactionAsync(IsolationLevel.Serializable, ct);
        try
        {
            var result = await action();
            await transaction.CommitAsync(ct);
            return result;
        }
        catch (DbUpdateConcurrencyException ex)
        {
            await transaction.RollbackAsync(ct);
            throw new CharacterSyncConflictException(ex);
        }
    }
}
