namespace Eldoria.Core.Entities
{
    public class SceneCharacterSpell
    {
        public int Id { get; set; }

        public int SceneCharacterId { get; set; }
        public SceneCharacter SceneCharacter { get; set; } = null!;

        public int SpellId { get; set; }
        public Spell Spell { get; set; } = null!;
    }
}
