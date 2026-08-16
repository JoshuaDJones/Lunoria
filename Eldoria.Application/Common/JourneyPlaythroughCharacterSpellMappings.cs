using Eldoria.Application.Dtos;
using Eldoria.Core.Entities.Playthrough;

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
                JourneyPlaythroughCharacterId = Spell.JourneyPlaythroughCharacterId,
                SourceJourneyCharacterSpellId = Spell.JourneyCharacterSpellId,
                SnapshotSpellKey = Spell.SnapshotSpellKey
            };
        }
    }
}
