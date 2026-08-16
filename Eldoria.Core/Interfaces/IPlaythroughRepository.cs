using Eldoria.Core.Entities.Playthrough.Base;

namespace Eldoria.Core.Interfaces;

public interface IPlaythroughRepository
{
    Task<List<Playthrough>> ListForJourneyAsync(
        int userId,
        int sourceJourneyId,
        CancellationToken ct);
}
