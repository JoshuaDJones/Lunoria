namespace Eldoria.Application.Dtos
{
    public class JourneyPlaythroughCharacterSpellDto
    {
        public int Id { get; set; }
        public JourneyPlaythroughCharacterDto JourneyPlaythroughCharacter { get; set; } = null!;
        public JourneyCharacterSpellDto JourneyCharacterSpell { get; set; } = null!;
    }
}
