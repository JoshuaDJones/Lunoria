using Eldoria.Core.Entities;

namespace Eldoria.Core.Interfaces
{
    public interface IJourneyPlaythroughRepository : IRepository<JourneyPlaythrough>
    {
        Task<JourneyPlaythrough?> GetActiveForJourneyAsync(
            int userId,
            int journeyId,
            CancellationToken ct);

        Task<JourneyPlaythrough?> GetForUserAsync(
            int userId,
            int journeyId,
            int playthroughId,
            CancellationToken ct);

        Task<List<JourneyPlaythrough>> ListForJourneyAsync(
            int userId,
            int journeyId,
            int skip,
            int take,
            CancellationToken ct);
    }
}
