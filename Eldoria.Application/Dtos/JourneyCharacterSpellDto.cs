namespace Eldoria.Application.Dtos
{
    public class JourneyCharacterSpellDto
    {
        public int Id { get; set; }
        public int JourneyCharacterId { get; set; }
        public int SpellId { get; set; }
        public SpellDto Spell { get; set; } = null!;
    }
}
