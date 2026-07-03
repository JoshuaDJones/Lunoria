using Eldoria.Core.Entities;

namespace Eldoria.Core.Interfaces
{
    public interface IJourneyCharacterSpellRepository
        : IRepository<JourneyCharacterSpell>
    {
        Task<JourneyCharacter?> GetCharacterForUserAsync(
            int userId,
            int journeyCharacterId,
            CancellationToken ct);

        Task<JourneyCharacterSpell?> GetAssignmentForUserAsync(
            int userId,
            int journeyCharacterId,
            int spellId,
            CancellationToken ct);
    }
}
