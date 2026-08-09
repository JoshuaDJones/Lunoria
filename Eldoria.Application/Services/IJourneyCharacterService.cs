using Eldoria.Application.Common;

namespace Eldoria.Application.Services
{
    public interface IJourneyCharacterService
    {
        Task<Result> ReplaceJourneyCharacters(int userId, int journeyId, List<int> characterIds, CancellationToken ct);
        Task<Result> DeleteAsync(int userId, int journeyCharacterId, CancellationToken ct);
        Task<Result<Eldoria.Application.Dtos.JourneyCharacterDto>> UpdateAsync(int userId, int journeyCharacterId, int? meleeAttackDamage, int? bowAttackDamage, int movement, int maxConsumableInventory, int maxEquippableInventory, int maxHp, int maxMp, bool isInitiallyActive, int? alternateFormId, CancellationToken ct);
    }
}
