using Eldoria.Application.Common;
using Eldoria.Application.Dtos;
using Eldoria.Core.Entities;
using Eldoria.Core.Interfaces;

namespace Eldoria.Application.Services
{
    public class SceneCharacterService(
        ISceneCharacterRepository sceneCharacterRepository,
        IRepository<SceneCharacterSpell> sceneCharacterSpellRepository,
        IOwnershipRepository ownershipRepository,
        ICharacterRepository characterRepository,
        ISpellRepository spellRepository) : ISceneCharacterService
    {
        private readonly ISceneCharacterRepository _sceneCharacterRepository = sceneCharacterRepository;
        private readonly IRepository<SceneCharacterSpell> _sceneCharacterSpellRepository = sceneCharacterSpellRepository;
        private readonly IOwnershipRepository _ownershipRepository = ownershipRepository;
        private readonly ICharacterRepository _characterRepository = characterRepository;
        private readonly ISpellRepository _spellRepository = spellRepository;

        public async Task<Result<List<SceneCharacterDto>>> ListAsync(int userId, int sceneId, CancellationToken ct)
        {
            if (await _ownershipRepository.GetSceneAsync(userId, sceneId, ct) is null)
                return Result<List<SceneCharacterDto>>.Fail(SceneNotFound);

            var characters = await _sceneCharacterRepository.ListForSceneAsync(userId, sceneId, ct);
            return Result<List<SceneCharacterDto>>.Ok(characters.Select(character => character.ToDto()).ToList());
        }

        public async Task<Result<SceneCharacterDto>> GetAsync(int userId, int sceneCharacterId, CancellationToken ct)
        {
            var character = await _sceneCharacterRepository.GetForUserAsync(userId, sceneCharacterId, ct);
            return character is null
                ? Result<SceneCharacterDto>.Fail(SceneCharacterNotFound)
                : Result<SceneCharacterDto>.Ok(character.ToDto());
        }

        public async Task<Result<SceneCharacterDto>> AddSceneCharacterAsync(
            int userId,
            int sceneId,
            int characterId,
            CancellationToken ct)
        {
            if (await _ownershipRepository.GetSceneAsync(userId, sceneId, ct) is null)
                return Result<SceneCharacterDto>.Fail(SceneNotFound);

            var character = await _characterRepository.GetByIdForUserAsync(userId, characterId, ct);
            if (character is null)
                return Result<SceneCharacterDto>.Fail(CharacterNotFound);

            var existing = await _sceneCharacterRepository.ListForSceneAsync(userId, sceneId, ct);
            if (existing.Any(item => item.CharacterId == characterId))
                return Result<SceneCharacterDto>.Fail(new Error("SceneCharacter.Duplicate", "The character is already attached to this scene."));

            var sceneCharacter = new SceneCharacter
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
                Character = character,
                AlternateFormId = character.BaseAlternateFormId,
                AlternateForm = character.BaseAlternateForm
            };

            await _sceneCharacterRepository.AddAsync(sceneCharacter, ct);
            await _sceneCharacterRepository.SaveChangesAsync(ct);
            return Result<SceneCharacterDto>.Ok(sceneCharacter.ToDto());
        }

        public async Task<Result<SceneCharacterDto>> UpdateAsync(
            int userId,
            int sceneCharacterId,
            int? meleeAttackDamage,
            int? bowAttackDamage,
            int movement,
            int maxConsumableInventory,
            int maxEquippableInventory,
            int maxHp,
            int maxMp,
            bool isInitiallyActive,
            int? alternateFormId,
            CancellationToken ct)
        {
            var sceneCharacter = await _sceneCharacterRepository.GetForUserAsync(userId, sceneCharacterId, ct);
            if (sceneCharacter is null)
                return Result<SceneCharacterDto>.Fail(SceneCharacterNotFound);

            var validationError = ValidateStats(meleeAttackDamage, bowAttackDamage, movement, maxConsumableInventory, maxEquippableInventory, maxHp, maxMp);
            if (validationError is not null)
                return Result<SceneCharacterDto>.Fail(validationError);

            Character? alternateForm = null;
            if (alternateFormId is not null)
            {
                alternateForm = await _characterRepository.GetByIdForUserAsync(userId, alternateFormId.Value, ct);
                if (alternateForm is null || alternateForm.Id == sceneCharacter.CharacterId)
                    return Result<SceneCharacterDto>.Fail(new Error("SceneCharacter.InvalidAlternateForm", "The alternate form is invalid."));
            }

            sceneCharacter.MeleeAttackDamage = meleeAttackDamage;
            sceneCharacter.BowAttackDamage = bowAttackDamage;
            sceneCharacter.Movement = movement;
            sceneCharacter.MaxConsumableInventory = maxConsumableInventory;
            sceneCharacter.MaxEquippableInventory = maxEquippableInventory;
            sceneCharacter.MaxHp = maxHp;
            sceneCharacter.MaxMp = maxMp;
            sceneCharacter.IsInitiallyActive = isInitiallyActive;
            sceneCharacter.AlternateFormId = alternateFormId;
            sceneCharacter.AlternateForm = alternateForm;

            await _sceneCharacterRepository.SaveChangesAsync(ct);
            return Result<SceneCharacterDto>.Ok(sceneCharacter.ToDto());
        }

        public async Task<Result<SceneCharacterDto>> ReplaceSpellsAsync(
            int userId,
            int sceneCharacterId,
            IReadOnlyCollection<int> spellIds,
            CancellationToken ct)
        {
            var sceneCharacter = await _sceneCharacterRepository.GetForUserAsync(userId, sceneCharacterId, ct);
            if (sceneCharacter is null)
                return Result<SceneCharacterDto>.Fail(SceneCharacterNotFound);

            var distinctIds = spellIds.Distinct().ToList();
            var spells = await _spellRepository.GetSpellsByIdsForUserAsync(userId, distinctIds, ct);
            if (spells.Count != distinctIds.Count)
                return Result<SceneCharacterDto>.Fail(new Error("Spell.NotFound", "One or more spells were not found or are not owned by the current user."));

            foreach (var assignment in sceneCharacter.SceneCharacterSpells.ToList())
                _sceneCharacterSpellRepository.Remove(assignment);

            sceneCharacter.SceneCharacterSpells.Clear();
            foreach (var spell in spells)
            {
                var assignment = new SceneCharacterSpell { SceneCharacterId = sceneCharacterId, SpellId = spell.Id, Spell = spell };
                sceneCharacter.SceneCharacterSpells.Add(assignment);
                await _sceneCharacterSpellRepository.AddAsync(assignment, ct);
            }

            await _sceneCharacterSpellRepository.SaveChangesAsync(ct);
            return Result<SceneCharacterDto>.Ok(sceneCharacter.ToDto());
        }

        public async Task<Result> DeleteSceneCharacterAsync(int userId, int sceneCharacterId, CancellationToken ct)
        {
            var character = await _sceneCharacterRepository.GetForUserAsync(userId, sceneCharacterId, ct);
            if (character is null)
                return Result.Fail(SceneCharacterNotFound);

            _sceneCharacterRepository.Remove(character);
            await _sceneCharacterRepository.SaveChangesAsync(ct);
            return Result.Ok();
        }

        private static Error? ValidateStats(int? melee, int? bow, int movement, int consumables, int equipment, int maxHp, int maxMp)
        {
            if (melee < 0 || bow < 0 || movement < 0 || consumables < 0 || equipment < 0 || maxHp < 1 || maxMp < 0)
                return new Error("SceneCharacter.InvalidStats", "Scene character statistics contain an invalid value.");
            return null;
        }

        private static readonly Error SceneNotFound = new("Scene.NotFound", "Scene was not found.");
        private static readonly Error CharacterNotFound = new("Character.NotFound", "Character was not found.");
        private static readonly Error SceneCharacterNotFound = new("SceneCharacter.NotFound", "Scene character was not found.");
    }
}
