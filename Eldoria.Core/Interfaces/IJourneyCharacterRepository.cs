using Eldoria.Core.Entities;

namespace Eldoria.Core.Interfaces
{
    public interface IJourneyCharacterRepository: IRepository<JourneyCharacter>
    {
        Task<List<JourneyCharacter>> GetJourneyCharacters(int journeyId, CancellationToken ct);
        Task<JourneyCharacter?> GetForUserAsync(int userId, int journeyCharacterId, CancellationToken ct);
    }
}
