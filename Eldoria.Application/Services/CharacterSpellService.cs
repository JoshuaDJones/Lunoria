using Eldoria.Application.Common;
using Eldoria.Core.Interfaces;

namespace Eldoria.Application.Services
{
    public class CharacterSpellService : ICharacterSpellService
    {
        private readonly ICharacterSpellRepository _characterSpellRepository;
        private readonly ICharacterRepository _characterRepository;
        private readonly ISpellRepository _spellRepository;

        public CharacterSpellService(ICharacterSpellRepository characterSpellRepository, ICharacterRepository characterRepository, ISpellRepository spellRepository)
        {
            _characterSpellRepository = characterSpellRepository;
            _characterRepository = characterRepository;
            _spellRepository = spellRepository;
        }

        public async Task<Result> ReplaceCharacterSpells(int characterId, List<int> spellIds, CancellationToken ct)
        {
            var character = await _characterRepository.GetByIdAsync(characterId, ct);

            if (character is null)
                return Result.Fail(new Error("Character.NotFound", "Character was not found."));

            var spells = await _spellRepository.GetSpellsByIds(spellIds, ct);

            var missingIds = spellIds.Where((id, index) => spells[index] is null).ToList();

            if (missingIds.Count > 0)
                return Result.Fail(new Error("Spell.NotFound", $"One or more of your spell ids do not exist. ids: $\"One or more of your spell ids do not exist. ids: {string.Join(", ", missingIds)}"));

            await _characterSpellRepository.RemoveCharacterSpells(characterId, ct);
            await _characterSpellRepository.AddCharacterSpells(spellIds, characterId, ct);

            return Result.Ok();
        }
    }
}
