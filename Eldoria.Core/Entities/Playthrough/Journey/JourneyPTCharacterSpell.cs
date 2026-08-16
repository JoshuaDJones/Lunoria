using Eldoria.Core.Entities.Playthrough.Base;

namespace Eldoria.Core.Entities.Playthrough.Journey
{
    public class JourneyPTCharacterSpell
    {
        public int Id { get; set; }
        public int? SourceJourneyCharacterSpellId { get; set; }

        public int JourneyPTCharacterId { get; set; }
        public JourneyPTCharacter JourneyPTCharacter { get; set; } = null!;

        public int PlaythroughSpellId { get; set; }
        public PlaythroughSpell PlaythroughSpell { get; set; } = null!;
    }
}
