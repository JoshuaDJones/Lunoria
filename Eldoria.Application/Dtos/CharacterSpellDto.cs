namespace Eldoria.Application.Dtos
{
    public class CharacterSpellDto
    {
        public int Id { get; set; }
        public int CharacterId { get; set; }
        public SpellDto Spell { get; set; } = null!;
    }
}
