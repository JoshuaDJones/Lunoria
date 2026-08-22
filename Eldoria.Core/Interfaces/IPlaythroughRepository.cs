using Eldoria.Core.Entities.Playthrough.Base;

namespace Eldoria.Core.Interfaces;

public interface IPlaythroughRepository
{
    Task<IPlaythroughTransaction> BeginStartTransactionAsync(CancellationToken ct);

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

    Task AddAsync(Playthrough playthrough, CancellationToken ct);

    Task<int> SaveChangesAsync(CancellationToken ct);
}
