namespace Eldoria.Core.Interfaces;

public interface ICharacterSyncUnitOfWork
{
    Task<T> ExecuteAsync<T>(Func<Task<T>> action, CancellationToken ct);
}
