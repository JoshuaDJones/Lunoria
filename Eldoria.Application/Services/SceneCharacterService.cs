using Eldoria.Application.Common;
using Eldoria.Core.Entities;
using Eldoria.Core.Interfaces;

namespace Eldoria.Application.Services
{
    public class SceneCharacterService(
        IRepository<SceneCharacter> sceneCharacterRepository,
        IOwnershipRepository ownershipRepository,
        ICharacterRepository characterRepository) : ISceneCharacterService
    {
        private readonly IRepository<SceneCharacter> _sceneCharacterRepository = sceneCharacterRepository;
        private readonly IOwnershipRepository _ownershipRepository = ownershipRepository;
        private readonly ICharacterRepository _characterRepository = characterRepository;

        public async Task<Result> AddSceneCharacterAsync(
            int userId,
            int sceneId,
            int characterId,
            CancellationToken ct)
        {
            if (await _ownershipRepository.GetSceneAsync(userId, sceneId, ct) is null)
                return Result.Fail(new Error("Scene.NotFound", "Scene was not found."));

            var character = await _characterRepository.GetByIdForUserAsync(userId, characterId, ct);
            if (character is null)
                return Result.Fail(new Error("Character.NotFound", "Character was not found."));

            await _sceneCharacterRepository.AddAsync(new SceneCharacter
            {
                MaxHp = character.BaseMaxHp,
                MaxMp = character.BaseMaxMp,
                MeleeAttackDamage = character.BaseMeleeAttackDamage,
                BowAttackDamage = character.BaseBowAttackDamage,
                Movement = character.BaseMovement,
                MaxConsumableInventory = character.BaseMaxConsumableInventory,
                MaxEquippableInventory = character.BaseMaxEquippableInventory,
                IsInitiallyActive = true,
                SceneId = sceneId,
                CharacterId = characterId,
            }, ct);
            await _sceneCharacterRepository.SaveChangesAsync(ct);
            return Result.Ok();
        }

        public async Task<Result> DeleteSceneCharacterAsync(
            int userId,
            int sceneCharacterId,
            CancellationToken ct)
        {
            var character = await _ownershipRepository.GetSceneCharacterAsync(userId, sceneCharacterId, ct);
            if (character is null)
                return Result.Fail(new Error("SceneCharacter.NotFound", "Scene character was not found."));

            _sceneCharacterRepository.Remove(character);
            await _sceneCharacterRepository.SaveChangesAsync(ct);
            return Result.Ok();
        }
    }
}
