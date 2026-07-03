using Eldoria.Core.Entities;

namespace Eldoria.Core.Interfaces
{
    public interface IJourneyCharacterEquipmentRepository
        : IRepository<JourneyCharacterEquippableItem>
    {
        Task<JourneyCharacter?> GetCharacterForUserAsync(
            int userId,
            int journeyCharacterId,
            CancellationToken ct);

        Task<JourneyCharacterEquippableItem?> GetAssignmentForUserAsync(
            int userId,
            int assignmentId,
            CancellationToken ct);
    }
}
