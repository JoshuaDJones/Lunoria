using Eldoria.Application.Common;
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

        public async Task<Result> UpdateJourneyCharacter(
            int userId,
            int journeyCharacterId,
            int newHp,
            int newMp,
            bool isAlternateForm,
            CancellationToken ct)
        {
            var character = await _ownershipRepository.GetJourneyCharacterAsync(
                userId,
                journeyCharacterId,
                ct);

            if (character is null)
                return Result.Fail(new Error("JourneyCharacter.NotFound", "Journey character was not found."));

            character.CurrentHp = newHp;
            character.CurrentMp = newMp;
            character.IsInAlternateForm = isAlternateForm;
            await _journeyCharacterRepository.SaveChangesAsync(ct);
            return Result.Ok();
        }

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
            foreach (var journeyCharacter in journeyCharacters)
                _journeyCharacterRepository.Remove(journeyCharacter);

            foreach (var character in selectedCharacters)
            {
                await _journeyCharacterRepository.AddAsync(new JourneyCharacter
                {
                    CurrentHp = character.BaseMaxHp,
                    CurrentMp = character.BaseMaxMp,
                    MaxHp = character.BaseMaxHp,
                    MaxMp = character.BaseMaxMp,
                    MeleeAttackDamage = character.BaseMeleeAttackDamage,
                    BowAttackDamage = character.BaseBowAttackDamage,
                    Movement = character.BaseMovement,
                    MaxConsumableInventory = character.BaseMaxConsumableInventory,
                    MaxEquippableInventory = character.BaseMaxEquippableInventory,
                    IsDown = false,
                    IsInAlternateForm = false,
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
