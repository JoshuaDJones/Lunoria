using Eldoria.Application.Dtos;
using Eldoria.Core.Entities;

namespace Eldoria.Application.Common
{
    public static class JourneyPlaythroughCharacterSpellMappings
    {
        public static JourneyPlaythroughCharacterSpellDto ToDto(
            this JourneyPlaythroughCharacterSpell Spell)
        {
            return new JourneyPlaythroughCharacterSpellDto
            {
                Id = Spell.Id,
                JourneyPlaythroughCharacter = Spell.JourneyPlaythroughCharacter.ToDto(),
                JourneyCharacterSpell = Spell.JourneyCharacterSpell.ToDto()
            };
        }
    }
}
