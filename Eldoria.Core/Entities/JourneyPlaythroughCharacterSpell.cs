namespace Eldoria.Core.Entities
{
    public class JourneyPlaythroughCharacterSpell
    {
        public int Id { get; set; }

        public int JourneyPlaythroughCharacterId { get; set; }
        public JourneyPlaythroughCharacter JourneyPlaythroughCharacter { get; set; } = null!;

        public int JourneyCharacterSpellId { get; set; }
        public JourneyCharacterSpell JourneyCharacterSpell { get; set; } = null!;
    }
}
