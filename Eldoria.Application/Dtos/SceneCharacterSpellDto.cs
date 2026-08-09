namespace Eldoria.Application.Dtos
{
    public class SceneCharacterSpellDto
    {
        public int Id { get; set; }
        public int SceneCharacterId { get; set; }
        public SpellDto Spell { get; set; } = null!;
    }
}
