using Eldoria.Application.Common;
using Eldoria.Application.Dtos;

namespace Eldoria.Application.Services
{
    public interface ISceneCharacterService
    {
        Task<Result<List<SceneCharacterDto>>> ListAsync(int userId, int sceneId, CancellationToken ct);
        Task<Result<SceneCharacterDto>> GetAsync(int userId, int sceneCharacterId, CancellationToken ct);
        Task<Result<SceneCharacterDto>> AddSceneCharacterAsync(int userId, int sceneId, int characterId, CancellationToken ct);
        Task<Result<SceneCharacterDto>> UpdateAsync(int userId, int sceneCharacterId, int? meleeAttackDamage, int? bowAttackDamage, int movement, int maxConsumableInventory, int maxEquippableInventory, int maxHp, int maxMp, bool isInitiallyActive, int? alternateFormId, CancellationToken ct);
        Task<Result<SceneCharacterDto>> ReplaceSpellsAsync(int userId, int sceneCharacterId, IReadOnlyCollection<int> spellIds, CancellationToken ct);
        Task<Result> DeleteSceneCharacterAsync(int userId, int sceneCharacterId, CancellationToken ct);
    }
}
