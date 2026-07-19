namespace Eldoria.Core.Entities
{
    public class JourneyCharacterSpell
    {
        public int Id { get; set; }

        public int JourneyCharacterId { get; set; }
        public JourneyCharacter JourneyCharacter { get; set; } = null!;

        public int SpellId { get; set; }
        public Spell Spell { get; set; } = null!;
    }
}
