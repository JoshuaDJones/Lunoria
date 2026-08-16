namespace Eldoria.Core.Entities.Playthrough.Base
{
    public class PlaythroughCharacterSpell
    {
        public int Id { get; set; }
        public int SourceCharacterSpellId { get; set; }

        public int PlaythroughCharacterId { get; set; }
        public PlaythroughCharacter PlaythroughCharacter { get; set; } = null!;

        public int PlaythroughSpellId { get; set; }
        public PlaythroughSpell PlaythroughSpell { get; set; } = null!;
    }
}
