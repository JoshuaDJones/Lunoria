using Eldoria.Application.Common;
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

            await _characterSpellRepository.RemoveCharacterSpells(characterId, ct);
            await _characterSpellRepository.AddCharacterSpells(distinctSpellIds, characterId, ct);
            return Result.Ok();
        }
    }
}
