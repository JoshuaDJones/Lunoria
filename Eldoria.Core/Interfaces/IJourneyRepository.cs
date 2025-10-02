using Eldoria.Core.Entities;

namespace Eldoria.Core.Interfaces
{
    public interface IJourneyRepository : IRepository<Journey>
    { 
        Task<List<Journey>> GetUsersJourneys(int userId, int skip, int take, CancellationToken ct);
        Task<Journey?> GetJourneyWithPlayers(int journeyId, CancellationToken ct);
    }
}
