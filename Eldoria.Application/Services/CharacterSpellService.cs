using Eldoria.Application.Common;
using Eldoria.Core.Entities;
using Eldoria.Core.Interfaces;

namespace Eldoria.Application.Services
{
    public class CharacterSpellService(
        ICharacterSpellRepository characterSpellRepository,
        ICharacterRepository characterRepository,
        ISpellRepository spellRepository) : ICharacterSpellService
    {
        private readonly ICharacterSpellRepository _characterSpellRepository = characterSpellRepository;
        private readonly ICharacterRepository _characterRepository = characterRepository;
        private readonly ISpellRepository _spellRepository = spellRepository;

        public async Task<Result> ReplaceCharacterSpells(
            int userId,
            int characterId,
            List<int> spellIds,
            CancellationToken ct)
        {
            var character = await _characterRepository.GetByIdForUserAsync(userId, characterId, ct);
            if (character is null)
                return Result.Fail(new Error("Character.NotFound", "Character was not found."));

            var distinctSpellIds = spellIds.Distinct().ToList();
            var spells = await _spellRepository.GetSpellsByIdsForUserAsync(
                userId,
                distinctSpellIds,
                ct);

            if (spells.Count != distinctSpellIds.Count)
                return Result.Fail(new Error(
                    "Spell.NotFound",
                    "One or more spells were not found or are not owned by the current user."));

            if (spells.Any(spell => spell.IsDeleted &&
                !character.CharacterSpells.Any(link => link.SpellId == spell.Id)))
                return Result.Fail(new Error("Spell.Archived", "Archived spells cannot be newly assigned."));

            // Keep unchanged links; no-op saves must not advance the source revision.
            foreach (var link in character.CharacterSpells.Where(link => !distinctSpellIds.Contains(link.SpellId)).ToList())
                _characterSpellRepository.Remove(link);

            var existingIds = character.CharacterSpells.Select(link => link.SpellId).ToHashSet();

            foreach (var spell in spells.Where(spell => !existingIds.Contains(spell.Id)))
                await _characterSpellRepository.AddAsync(new CharacterSpell
                {
                    CharacterId = characterId,
                    SpellId = spell.Id,
                    Spell = spell
                }, ct);

            await _characterSpellRepository.SaveChangesAsync(ct);
            return Result.Ok();
        }
    }
}
