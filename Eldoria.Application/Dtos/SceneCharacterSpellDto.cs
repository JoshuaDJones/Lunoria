namespace Eldoria.Application.Dtos
{
    public class SceneCharacterSpellDto
    {
        public int Id { get; set; }
        public SceneCharacterDto SceneCharacter { get; set; } = null!;
        public SpellDto Spell { get; set; } = null!;
    }
}
