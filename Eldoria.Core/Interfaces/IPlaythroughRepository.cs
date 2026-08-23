using Eldoria.Core.Entities.Playthrough.Base;
using Eldoria.Core.Entities.Playthrough.Scene;

namespace Eldoria.Core.Interfaces;

public interface IPlaythroughRepository
{
    Task<IPlaythroughTransaction> BeginStartTransactionAsync(CancellationToken ct);

    Task<IPlaythroughTransaction> BeginSceneStartTransactionAsync(CancellationToken ct);

    Task<PlaythroughStartAssets> GetStartAssetsAsync(
        int userId,
        IReadOnlyCollection<int> referencedCharacterIds,
        CancellationToken ct);

    Task<List<Playthrough>> ListUnfinishedForJourneyAsync(
        int userId,
        int sourceJourneyId,
        CancellationToken ct);

    Task<List<Playthrough>> ListForJourneyAsync(
        int userId,
        int sourceJourneyId,
        CancellationToken ct);

    Task<Playthrough?> GetDetailsAsync(
        int userId,
        int playthroughId,
        CancellationToken ct);

    Task<ScenePT?> GetSceneForStartAsync(
        int userId,
        int playthroughId,
        int sceneId,
        CancellationToken ct);

    Task<ScenePT?> GetSceneForEndAsync(
        int userId,
        int playthroughId,
        int sceneId,
        CancellationToken ct);

    Task<ScenePT?> GetSceneDetailsAsync(
        int userId,
        int playthroughId,
        int sceneId,
        CancellationToken ct);

    Task<ScenePT?> GetSceneForCharacterInstanceAddAsync(
        int userId,
        int playthroughId,
        int sceneId,
        CancellationToken ct);

    Task AddAsync(Playthrough playthrough, CancellationToken ct);

    Task<int> SaveChangesAsync(CancellationToken ct);
}
