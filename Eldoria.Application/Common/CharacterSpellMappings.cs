using Eldoria.Application.Dtos;
using Eldoria.Core.Entities;

namespace Eldoria.Application.Common
{
    public static class CharacterSpellMappings
    {
        public static CharacterSpellDto ToDto(this CharacterSpell characterSpell)
        {
            return new CharacterSpellDto
            {
                Id = characterSpell.Id,
                CharacterId = characterSpell.CharacterId,
                Spell = characterSpell.Spell.ToDto(),
            };
        }
    }
}
