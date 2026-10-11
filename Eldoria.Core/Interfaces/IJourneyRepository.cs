using Eldoria.Core.Entities;

namespace Eldoria.Core.Interfaces
{
    public interface IJourneyRepository : IRepository<Journey>
    { 
        Task<Journey?> GetCharacterSyncSummarySourceAsync(int userId, int journeyId, CancellationToken ct);
        Task AddWithNextSortOrderAsync(Journey journey, CancellationToken ct);
        Task<List<Journey>> GetUsersJourneys(int userId, int skip, int take, CancellationToken ct);
        Task<Journey?> GetJourneyWithPlayers(int journeyId, CancellationToken ct);
        Task<Journey?> GetPlaythroughSourceAsync(
            int userId,
            int journeyId,
            CancellationToken ct);
    }
}
