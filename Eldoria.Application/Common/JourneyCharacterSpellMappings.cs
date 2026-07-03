using Eldoria.Application.Dtos;
using Eldoria.Core.Entities;

namespace Eldoria.Application.Common
{
    public static class JourneyCharacterSpellMappings
    {
        public static JourneyCharacterSpellDto ToDto(this JourneyCharacterSpell spell)
        {
            return new JourneyCharacterSpellDto
            {
                Id = spell.Id,
                JourneyCharacterId = spell.JourneyCharacterId,
                SpellId = spell.SpellId,
                Spell = spell.Spell.ToDto(),
            };
        }
    }
}
