using Eldoria.Core.Entities;

namespace Eldoria.Core.Interfaces
{
    public interface IJourneyPlaythroughCharacterRepository : IRepository<JourneyPlaythroughCharacter>
    {
        Task<List<JourneyPlaythroughCharacter>> ListForPlaythroughAsync(int userId, int journeyPlaythroughId, CancellationToken ct);
        Task<JourneyPlaythroughCharacter?> GetForUserAsync(int userId, int characterId, CancellationToken ct);
    }
}
