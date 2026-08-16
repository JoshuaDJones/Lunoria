using Eldoria.Application.Common;
using Eldoria.Application.Dtos;
using Eldoria.Core.Entities;
using Eldoria.Core.Interfaces;

namespace Eldoria.Application.Services
{
    public class JourneyCharacterService(
        IJourneyCharacterRepository journeyCharacterRepository,
        IOwnershipRepository ownershipRepository,
        ICharacterRepository characterRepository) : IJourneyCharacterService
    {
        private readonly IJourneyCharacterRepository _journeyCharacterRepository = journeyCharacterRepository;
        private readonly IOwnershipRepository _ownershipRepository = ownershipRepository;
        private readonly ICharacterRepository _characterRepository = characterRepository;

        public async Task<Result> DeleteAsync(
            int userId,
            int journeyCharacterId,
            CancellationToken ct)
        {
            var character = await _ownershipRepository.GetJourneyCharacterAsync(
                userId,
                journeyCharacterId,
                ct);

            if (character is null)
                return Result.Fail(new Error("JourneyCharacter.NotFound", "Journey character was not found."));

            _journeyCharacterRepository.Remove(character);
            await _journeyCharacterRepository.SaveChangesAsync(ct);
            return Result.Ok();
        }

        public async Task<Result<JourneyCharacterDto>> UpdateAsync(
            int userId, int journeyCharacterId,
            int? meleeAttackDamage, int? bowAttackDamage, int movement,
            int maxConsumableInventory, int maxEquippableInventory,
            int maxHp, int maxMp, bool isInitiallyActive,
            int? alternateFormId, CancellationToken ct)
        {
            var journeyCharacter = await _journeyCharacterRepository.GetForUserAsync(userId, journeyCharacterId, ct);
            if (journeyCharacter is null)
                return Result<JourneyCharacterDto>.Fail(new Error("JourneyCharacter.NotFound", "Journey character was not found."));

            if (meleeAttackDamage < 0 || bowAttackDamage < 0 || movement < 0 || maxConsumableInventory < 0 || maxEquippableInventory < 0 || maxHp < 1 || maxMp < 0)
                return Result<JourneyCharacterDto>.Fail(new Error("JourneyCharacter.InvalidStats", "Journey character statistics contain an invalid value."));

            Character? alternateForm = null;
            if (alternateFormId is not null)
            {
                alternateForm = await _characterRepository.GetByIdForUserAsync(userId, alternateFormId.Value, ct);
                if (alternateForm is null || alternateForm.Id == journeyCharacter.CharacterId)
                    return Result<JourneyCharacterDto>.Fail(new Error("JourneyCharacter.InvalidAlternateForm", "The alternate form is invalid."));
            }

            journeyCharacter.MeleeAttackDamage = meleeAttackDamage;
            journeyCharacter.BowAttackDamage = bowAttackDamage;
            journeyCharacter.Movement = movement;
            journeyCharacter.MaxConsumableInventory = maxConsumableInventory;
            journeyCharacter.MaxEquippableInventory = maxEquippableInventory;
            journeyCharacter.MaxHp = maxHp;
            journeyCharacter.MaxMp = maxMp;
            journeyCharacter.IsInitiallyActive = isInitiallyActive;
            journeyCharacter.AlternateFormId = alternateFormId;
            journeyCharacter.AlternateForm = alternateForm;

            await _journeyCharacterRepository.SaveChangesAsync(ct);
            return Result<JourneyCharacterDto>.Ok(journeyCharacter.ToDto());
        }

        public async Task<Result> ReplaceJourneyCharacters(
            int userId,
            int journeyId,
            List<int> characterIds,
            CancellationToken ct)
        {
            if (await _ownershipRepository.GetJourneyAsync(userId, journeyId, ct) is null)
                return Result.Fail(new Error("Journey.NotFound", "Journey was not found."));

            var selectedCharacters = new List<Character>();
            var missingCharacterIds = new List<int>();

            foreach (var characterId in characterIds.Distinct())
            {
                var character = await _characterRepository.GetByIdForUserAsync(
                    userId,
                    characterId,
                    ct);

                if (character is null)
                    missingCharacterIds.Add(characterId);
                else
                    selectedCharacters.Add(character);
            }

            if (missingCharacterIds.Count > 0)
                return Result.Fail(new Error(
                    "Character.NotFound",
                    $"The following characters were not found or are not owned by the current user: {string.Join(", ", missingCharacterIds)}"));

            var journeyCharacters = await _journeyCharacterRepository.GetJourneyCharacters(journeyId, ct);
            var selectedCharacterIds = selectedCharacters
                .Select(character => character.Id)
                .ToHashSet();
            var journeyCharactersToRemove = journeyCharacters
                .Where(journeyCharacter =>
                    !selectedCharacterIds.Contains(journeyCharacter.CharacterId))
                .ToList();

            foreach (var journeyCharacter in journeyCharactersToRemove)
                _journeyCharacterRepository.Remove(journeyCharacter);

            var existingCharacterIds = journeyCharacters
                .Select(journeyCharacter => journeyCharacter.CharacterId)
                .ToHashSet();

            foreach (var character in selectedCharacters.Where(
                character => !existingCharacterIds.Contains(character.Id)))
            {
                await _journeyCharacterRepository.AddAsync(new JourneyCharacter
                {
                    MaxHp = character.BaseMaxHp,
                    MaxMp = character.BaseMaxMp,
                    MeleeAttackDamage = character.BaseMeleeAttackDamage,
                    BowAttackDamage = character.BaseBowAttackDamage,
                    Movement = character.BaseMovement,
                    MaxConsumableInventory = character.BaseMaxConsumableInventory,
                    MaxEquippableInventory = character.BaseMaxEquippableInventory,
                    IsInitiallyActive = true,
                    JourneyId = journeyId,
                    CharacterId = character.Id,
                    JourneyCharacterSpells = character.CharacterSpells
                        .Select(characterSpell => new JourneyCharacterSpell
                        {
                            SpellId = characterSpell.SpellId,
                        })
                        .ToList(),
                }, ct);
            }

            await _journeyCharacterRepository.SaveChangesAsync(ct);
            return Result.Ok();
        }
    }
}
