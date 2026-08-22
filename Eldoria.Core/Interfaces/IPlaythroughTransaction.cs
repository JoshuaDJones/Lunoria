namespace Eldoria.Core.Interfaces;

public interface IPlaythroughTransaction : IAsyncDisposable
{
    Task CommitAsync(CancellationToken ct);
}
