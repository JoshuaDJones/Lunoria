using Eldoria.Core.Entities.Playthrough.Base;

namespace Eldoria.Core.Interfaces;

public interface IPlaythroughJoinSessionRepository
{
    Task<Playthrough?> GetOwnedPlaythroughAsync(
        int userId,
        int playthroughId,
        CancellationToken ct);

    Task<PlaythroughJoinSession?> GetForOwnerAsync(
        int userId,
        int playthroughId,
        CancellationToken ct);

    Task<PlaythroughJoinSession?> GetValidByTokenHashAsync(
        string tokenHash,
        DateTime now,
        CancellationToken ct);

    Task<Playthrough?> GetPublicSnapshotAsync(
        string tokenHash,
        DateTime now,
        CancellationToken ct);

    Task AddAsync(PlaythroughJoinSession session, CancellationToken ct);
    Task<int> SaveChangesAsync(CancellationToken ct);
}
